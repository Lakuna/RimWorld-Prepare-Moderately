using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioMaker`.
	internal static class PawnFilterMaker {
		private const int _maximumRandomParts = 7;

		private const int _maximumRandomPartTries = 100;

		private const int _minimumRandomParts = 3;

		internal static IEnumerable<PawnFilterPartDef> AddableParts(PawnFilter filter) =>
			DefDatabase<PawnFilterPartDef>.AllDefs.Where((def) => filter.parts.Count((part) => part.def == def) < def.maxUses);

		internal static PawnFilterPart MakeFilterPart(PawnFilterPartDef def) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (def is null) {
				throw new ArgumentNullException(nameof(def));
			}
#else
			ArgumentNullException.ThrowIfNull(def);
#endif

			PawnFilterPart part = (PawnFilterPart)Activator.CreateInstance(def.filterPartClass);
			part.def = def;
			return part;
		}

		internal static PawnFilter Random(string seed) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (seed is null) {
				throw new ArgumentNullException(nameof(seed));
			}
#else
			ArgumentNullException.ThrowIfNull(seed);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnFilter filter = new PawnFilter {
				_category = PawnFilterCategory.CustomLocal,
				name = NameGenerator.GenerateName(RulePackDefOf.NamerScenario)
			};
#else
			PawnFilter filter = new() {
				_category = PawnFilterCategory.CustomLocal,
				name = NameGenerator.GenerateName(RulePackDefOf.NamerScenario)
			};
#endif

			Rand.PushState();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			Rand.Seed = seed.GetHashCode();
#else
			Rand.Seed = seed.GetHashCode(StringComparison.Ordinal);
#endif

			AddPartsFromCategory(filter, PawnFilterPartCategory.Normal, Rand.RangeInclusive(_minimumRandomParts, _maximumRandomParts));
			foreach (PawnFilterPart part in filter.parts) {
				part.Randomize();
			}

			for (int i = 0; i < filter.parts.Count; i++) {
				for (int j = 0; j < filter.parts.Count; j++) {
					if (i == j) {
						continue;
					}

					if (filter.parts.ElementAt(i).TryMerge(filter.parts.ElementAt(j))) {
						filter.RemovePart(filter.parts.ElementAt(j));
						j--;
						if (i > j) {
							i--;
						}
					}
				}
			}

			for (int i = 0; i < filter.parts.Count; i++) {
				for (int j = 0; j < filter.parts.Count; j++) {
					if (i == j) {
						continue;
					}

					if (!filter.parts.ElementAt(i).CanCoexistWith(filter.parts.ElementAt(j))) {
						filter.RemovePart(filter.parts.ElementAt(j));
						j--;
						if (i > j) {
							i--;
						}
					}
				}
			}

			foreach (string item in filter.ConfigErrors()) {
				PrepareModeratelyLogger.LogErrorMessage(item);
			}

			Rand.PopState();

			return filter;
		}

		private static void AddPartsFromCategory(PawnFilter filter, PawnFilterPartCategory category, int count) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<PawnFilterPart> parts = RandomPartsFromCategory(filter, category, count).ToList();
#else
			List<PawnFilterPart> parts = [.. RandomPartsFromCategory(filter, category, count)];
#endif
			foreach (PawnFilterPart part in parts) {
				filter.AddPart(part);
			}
		}

		private static bool CanAddPart(PawnFilter filter, PawnFilterPart part) =>
			filter is null ? throw new ArgumentNullException(nameof(filter))
			: part is null ? throw new ArgumentNullException(nameof(part))
			: filter.parts.All(part.CanCoexistWith);

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

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<PawnFilterPartDef> allowedParts = (from def in AddableParts(filter) where def.category == category select def).ToList();
#else
			List<PawnFilterPartDef> allowedParts = [.. from def in AddableParts(filter) where def.category == category select def];
#endif
			int yieldCount = 0;
			int tryCount = 0;
			while (yieldCount < count && allowedParts.Any()) {
				PawnFilterPart part = MakeFilterPart(allowedParts.RandomElementByWeight((def) => def.selectionWeight));
				if (CanAddPart(filter, part)) {
					yield return part;
					yieldCount++;
				}

				tryCount++;
				if (tryCount > _maximumRandomPartTries) {
					PrepareModeratelyLogger.LogErrorMessage("Failed to add filter part.");
					break;
				}
			}
		}
	}
}
