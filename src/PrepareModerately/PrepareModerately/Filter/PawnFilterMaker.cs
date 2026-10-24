using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioMaker`.
	public static class PawnFilterMaker {
		private const int _maximumRandomParts = 7;

		private const int _maximumRandomPartTries = 100;

		private const int _minimumRandomParts = 3;

		public static IEnumerable<PawnFilterPartDef> AddableParts(PawnFilter filter) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<PawnFilterPartDef>.AllDefs?.Where((def) => (filter.Parts?.Count((part) => part.Def == def) ?? 0) < def.MaxUses) ?? new List<PawnFilterPartDef>();
#else
			DefDatabase<PawnFilterPartDef>.AllDefs?.Where((def) => (filter.Parts?.Count((part) => part.Def == def) ?? 0) < def.MaxUses) ?? [];
#endif

		public static void AddPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add parts from category to null filter.");
				return;
			}

			foreach (PawnFilterPart part in RandomPartsFromCategory(filter, category, count)) {
				filter.AddPart(part);
			}
		}

		public static bool CanAddPart(PawnFilter filter, PawnFilterPart part) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(part is null)
#else
			part is not null
#endif
			&& (filter?.Parts?.All(part.CanCoexistWith) ?? false);

		public static PawnFilterPart MakeFilterPart(PawnFilterPartDef def) {
			if (def is null) {
				throw new InvalidOperationException("Attempted to create filter part from null definition.");
			}

			if (def.FilterPartClass is null) {
				throw new InvalidOperationException("Attempted to create filter part from definition with null filter part class.");
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilterPart part = ((PawnFilterPart)Activator.CreateInstance(def.FilterPartClass))
#else
			PawnFilterPart part = ((PawnFilterPart?)Activator.CreateInstance(def.FilterPartClass))
#endif
				?? throw new InvalidOperationException($"Failed to create an instance of a filter part for {def.defName}.");
			part.Def = def;
			return part;
		}

		public static PawnFilter Random(string seed) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilter filter = new PawnFilter() {
#else
			PawnFilter filter = new() {
#endif
				Category = PawnFilterCategory.CustomLocal,
				Name = RulePackDefOf.NamerScenario is null ? "null"
					: (NameGenerator.GenerateName(RulePackDefOf.NamerScenario) ?? "null")
			};
			if (filter.Parts is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to create random filter with null parts.");
				return filter;
			}

			if (seed is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to create random filter with null seed.");
				return filter;
			}

			Rand.PushState();

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			Rand.Seed = seed.GetHashCode();
#else
			Rand.Seed = seed.GetHashCode(StringComparison.Ordinal);
#endif
			AddPartsFromCategory(filter, PawnFilterPartCategory.Normal, Rand.RangeInclusive(_minimumRandomParts, _maximumRandomParts));
			foreach (PawnFilterPart part in filter.Parts) {
				part.Randomize();
			}

			for (int i = 0; i < filter.Parts.Count; i++) {
				for (int j = 0; j < filter.Parts.Count; j++) {
					if (i == j) {
						continue;
					}

					PawnFilterPart ej = filter.Parts[j];
					if (filter.Parts[i].TryMerge(ej)) {
						filter.RemovePart(ej);
						if (i > --j) {
							i--;
						}
					}
				}
			}

			for (int i = 0; i < filter.Parts.Count; i++) {
				for (int j = 0; j < filter.Parts.Count; j++) {
					if (i == j) {
						continue;
					}

					PawnFilterPart ej = filter.Parts[j];
					if (!filter.Parts[i].CanCoexistWith(ej)) {
						filter.RemovePart(ej);
						if (i > --j) {
							i--;
						}
					}
				}
			}

			foreach (string item in filter.ConfigErrors) {
				PrepareModeratelyLogger.LogErrorMessage(item);
			}

			Rand.PopState();

			return filter;
		}

		public static IEnumerable<PawnFilterPart> RandomPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
			if (count < 1) {
				yield break;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Collection<PawnFilterPartDef> allowedParts = new Collection<PawnFilterPartDef>(AddableParts(filter).Where((def) => def.Category == category).ToList());
#else
			Collection<PawnFilterPartDef> allowedParts = [.. AddableParts(filter).Where((def) => def.Category == category)];
#endif
			int yieldCount = 0;
			int tryCount = 0;
			while (yieldCount++ < count && allowedParts.Count > 0) {
				PawnFilterPart part = MakeFilterPart(allowedParts.RandomElementByWeight((def) => def.SelectionWeight));
				if (CanAddPart(filter, part)) {
					yield return part;
				}

				if (++tryCount > _maximumRandomPartTries) {
					PrepareModeratelyLogger.LogErrorMessage("Failed to add filter part.");
					break;
				}
			}
		}
	}
}
