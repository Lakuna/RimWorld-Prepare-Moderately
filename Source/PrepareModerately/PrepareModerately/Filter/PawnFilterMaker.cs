﻿using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class PawnFilterMaker {
		public static PawnFilter Filter { get; private set; }

		private const int MinimumRandomParts = 3;

		private const int MaximumRandomParts = 7;

		private const int MaximumRandomPartTries = 100;

		public static PawnFilter Random(string seed) {
			if (seed == null) {
				throw new ArgumentNullException(nameof(seed));
			}

			Filter = new PawnFilter {
				Category = PawnFilterCategory.CustomLocal,
				Name = NameGenerator.GenerateName(RulePackDefOf.NamerScenario),
				Description = null,
				Summary = null
			};

			Rand.PushState();
			Rand.Seed = seed.GetHashCode();
			AddPartsFromCategory(Filter, PawnFilterPartCategory.Normal, Rand.RangeInclusive(MinimumRandomParts, MaximumRandomParts));

			foreach (PawnFilterPart part in Filter.Parts) { part.Randomize(); }

			for (int i = 0; i < Filter.Parts.Count(); i++) {
				for (int j = 0; j < Filter.Parts.Count(); j++) {
					if (i == j) { continue; }
					if (Filter.Parts.ElementAt(i).TryMerge(Filter.Parts.ElementAt(j))) {
						Filter.RemovePart(Filter.Parts.ElementAt(j));
						j--;
						if (i > j) { i--; }
					}
				}
			}

			for (int i = 0; i < Filter.Parts.Count(); i++) {
				for (int j = 0; j < Filter.Parts.Count(); j++) {
					if (i == j) { continue; }
					if (!Filter.Parts.ElementAt(i).CanCoexistWith(Filter.Parts.ElementAt(j))) {
						Filter.RemovePart(Filter.Parts.ElementAt(j));
						j--;
						if (i > j) { i--; }
					}
				}
			}

			foreach (string item in Filter.ConfigErrors()) { Logger.LogErrorMessage(item); }

			Rand.PopState();

			PawnFilter result = Filter;
			Filter = null;
			return result;
		}

		private static void AddPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
			IEnumerable<PawnFilterPart> parts = RandomPartsFromCategory(filter, category, count);
			for (int i = 0; i < parts.Count(); i++) {
				_ = filter.Parts.Append(parts.ElementAt(i));
			}
		}

		private static IEnumerable<PawnFilterPart> RandomPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
			if (count <= 0) { yield break; }

			IEnumerable<PawnFilterPartDef> allowedParts = from def in AddableParts(filter) where def.Category == category select def;
			int yieldCount = 0;
			int tryCount = 0;
			while (yieldCount < count && allowedParts.Any()) {
				PawnFilterPart part = MakeFilterPart(allowedParts.RandomElementByWeight((PawnFilterPartDef def) => def.SelectionWeight));
				if (CanAddPart(filter, part)) {
					yield return part;
					yieldCount++;
				}

				tryCount++;
				if (tryCount > MaximumRandomPartTries) {
					Logger.LogErrorMessage("Failed to add filter part.");
					break;
				}
			}
		}

		public static IEnumerable<PawnFilterPartDef> AddableParts(PawnFilter filter) {
			return DefDatabase<PawnFilterPartDef>.AllDefsListForReading.Where((PawnFilterPartDef def) => filter.Parts.Count((PawnFilterPart part) => part.Def == def) < def.MaxUses);
		}

		private static bool CanAddPart(PawnFilter filter, PawnFilterPart part) => filter.Parts.All((PawnFilterPart existingPart) => part.CanCoexistWith(existingPart));

		public static PawnFilterPart MakeFilterPart(PawnFilterPartDef def) {
			if (def == null) {
				throw new ArgumentNullException(nameof(def));
			}

			PawnFilterPart part = (PawnFilterPart)Activator.CreateInstance(def.FilterPartClass);
			part.Def = def;
			return part;
		}
	}
}