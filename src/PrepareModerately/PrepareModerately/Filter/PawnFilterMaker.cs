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
		internal static PawnFilter Filter {
			get; private set;
		}

		private const int MaximumRandomParts = 7;

		private const int MaximumRandomPartTries = 100;

		private const int MinimumRandomParts = 3;

		internal static IEnumerable<PawnFilterPartDef> AddableParts(PawnFilter filter) =>
			DefDatabase<PawnFilterPartDef>.AllDefs.Where((def) => filter.PParts.Count((part) => part.PDef == def) < def.PMaxUses);

		internal static PawnFilterPart MakeFilterPart(PawnFilterPartDef def) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (def is null) {
				throw new ArgumentNullException(nameof(def));
			}
#else
			ArgumentNullException.ThrowIfNull(def);
#endif

			PawnFilterPart part = (PawnFilterPart)Activator.CreateInstance(def.PFilterPartClass);
			part.PDef = def;
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
			Filter = new PawnFilter {
				PCategory = PawnFilterCategory.CustomLocal,
				PName = NameGenerator.GenerateName(RulePackDefOf.NamerScenario),
				PDescription = null,
				PSummary = null
			};
#else
			Filter = new() {
				PCategory = PawnFilterCategory.CustomLocal,
				PName = NameGenerator.GenerateName(RulePackDefOf.NamerScenario),
				PDescription = null,
				PSummary = null
			};
#endif

			Rand.PushState();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			Rand.Seed = seed.GetHashCode();
#else
			Rand.Seed = seed.GetHashCode(StringComparison.Ordinal);
#endif

			AddPartsFromCategory(Filter, PawnFilterPartCategory.Normal, Rand.RangeInclusive(MinimumRandomParts, MaximumRandomParts));

			foreach (PawnFilterPart part in Filter.PParts) {
				part.randomize();
			}

			for (int i = 0; i < Filter.PParts.Count(); i++) {
				for (int j = 0; j < Filter.PParts.Count(); j++) {
					if (i == j) {
						continue;
					}

					if (Filter.PParts.ElementAt(i).tryMerge(Filter.PParts.ElementAt(j))) {
						Filter.RemovePart(Filter.PParts.ElementAt(j));
						j--;
						if (i > j) {
							i--;
						}
					}
				}
			}

			for (int i = 0; i < Filter.PParts.Count(); i++) {
				for (int j = 0; j < Filter.PParts.Count(); j++) {
					if (i == j) {
						continue;
					}

					if (!Filter.PParts.ElementAt(i).canCoexistWith(Filter.PParts.ElementAt(j))) {
						Filter.RemovePart(Filter.PParts.ElementAt(j));
						j--;
						if (i > j) {
							i--;
						}
					}
				}
			}

			foreach (string item in Filter.configErrors()) {
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

		private static bool CanAddPart(PawnFilter filter, PawnFilterPart part) =>
			filter is null ? throw new ArgumentNullException(nameof(filter))
			: part is null ? throw new ArgumentNullException(nameof(part))
			: filter.PParts.All(part.canCoexistWith);

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

			List<PawnFilterPartDef> allowedParts = (from def in AddableParts(filter) where def.PCategory == category select def).ToList();
			int yieldCount = 0;
			int tryCount = 0;
			while (yieldCount < count && allowedParts.Any()) {
				PawnFilterPart part = MakeFilterPart(allowedParts.RandomElementByWeight((def) => def.PSelectionWeight));
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
	}
}
