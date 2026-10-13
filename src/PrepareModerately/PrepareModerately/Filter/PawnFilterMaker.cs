using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioMaker`.
	public static class PawnFilterMaker {
		private static IEnumerable<PawnFilterPartDef> LegalPawnFilterParts => DefDatabase<PawnFilterPartDef>.AllDefs;

		public static PawnFilter Filter {
			get; private set;
		}

		private const int MinimumRandomParts = 3;

		private const int MaximumRandomParts = 7;

		private const int MaximumRandomPartTries = 100;

		public static PawnFilter Random(string seed) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (seed is null) {
				throw new ArgumentNullException(nameof(seed));
			}
#else
			ArgumentNullException.ThrowIfNull(seed);
#endif

			Filter = new PawnFilter {
				CategoryP = PawnFilterCategory.CustomLocal,
				NameP = NameGenerator.GenerateName(RulePackDefOf.NamerScenario),
				DescriptionP = null,
				SummaryP = null
			};

			Rand.PushState();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			Rand.Seed = seed.GetHashCode();
#else
			Rand.Seed = seed.GetHashCode(StringComparison.Ordinal);
#endif

			AddPartsFromCategory(Filter, PawnFilterPartCategory.Normal, Rand.RangeInclusive(MinimumRandomParts, MaximumRandomParts));

			foreach (PawnFilterPart part in Filter.PartsP) {
				part.Randomize();
			}

			for (int i = 0; i < Filter.PartsP.Count(); i++) {
				for (int j = 0; j < Filter.PartsP.Count(); j++) {
					if (i == j) {
						continue;
					}

					if (Filter.PartsP.ElementAt(i).TryMerge(Filter.PartsP.ElementAt(j))) {
						Filter.RemovePart(Filter.PartsP.ElementAt(j));
						j--;
						if (i > j) {
							i--;
						}
					}
				}
			}

			for (int i = 0; i < Filter.PartsP.Count(); i++) {
				for (int j = 0; j < Filter.PartsP.Count(); j++) {
					if (i == j) {
						continue;
					}

					if (!Filter.PartsP.ElementAt(i).CanCoexistWith(Filter.PartsP.ElementAt(j))) {
						Filter.RemovePart(Filter.PartsP.ElementAt(j));
						j--;
						if (i > j) {
							i--;
						}
					}
				}
			}

			foreach (string item in Filter.ConfigErrors()) {
				PrepareModeratelyLogger.LogErrorMessage(item);
			}

			Rand.PopState();

			PawnFilter result = Filter;
			Filter = null;
			return result;
		}

		private static void AddPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<PawnFilterPart> parts = RandomPartsFromCategory(filter, category, count).ToList();
#else
			List<PawnFilterPart> parts = [.. RandomPartsFromCategory(filter, category, count)];
#endif
			for (int i = 0; i < parts.Count; i++) { // Can't use `foreach` or the mouse stack will overflow when removing elements.
				filter.AddPart(parts.ElementAt(i));
			}
		}

		private static IEnumerable<PawnFilterPart> RandomPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			if (count <= 0) {
				yield break;
			}

			List<PawnFilterPartDef> allowedParts = (from def in AddableParts(filter) where def.category == category select def).ToList();
			int yieldCount = 0;
			int tryCount = 0;
			while (yieldCount < count && allowedParts.Any()) {
				PawnFilterPart part = MakeFilterPart(allowedParts.RandomElementByWeight((def) => def.selectionWeight));
				if (CanAddPart(filter, part)) {
					yield return part;
					yieldCount++;
				}

				tryCount++;
				if (tryCount > MaximumRandomPartTries) {
					PrepareModeratelyLogger.LogErrorMessage("Failed to add filter part.");
					break;
				}
			}
		}

		public static IEnumerable<PawnFilterPartDef> AddableParts(PawnFilter filter) => LegalPawnFilterParts.Where((def) => filter.PartsP.Count((part) => part.DefP == def) < def.maxUses);

		private static bool CanAddPart(PawnFilter filter, PawnFilterPart part) =>
			filter is null ? throw new ArgumentNullException(nameof(filter))
			: part is null ? throw new ArgumentNullException(nameof(part))
			: filter.PartsP.All(part.CanCoexistWith);

		public static PawnFilterPart MakeFilterPart(PawnFilterPartDef def) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (def is null) {
				throw new ArgumentNullException(nameof(def));
			}
#else
			ArgumentNullException.ThrowIfNull(def);
#endif

			PawnFilterPart part = (PawnFilterPart)Activator.CreateInstance(def.filterPartClass);
			part.DefP = def;
			return part;
		}
	}
}
