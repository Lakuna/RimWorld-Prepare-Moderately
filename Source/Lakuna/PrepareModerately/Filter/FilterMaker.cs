using System;
using System.Collections.Generic;
using System.Linq;
using Lakuna.PrepareModerately.Filter.FilterPart;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class FilterMaker {
		private static Filter filter;

		public static Filter GeneratingFilter => FilterMaker.filter;

		public static Filter GenerateNewRandomFilter(string seed) {
			FilterMaker.filter = new Filter();
			FilterMaker.filter.Category = FilterCategory.CustomLocal;
			FilterMaker.filter.name = NameGenerator.GenerateName(RulePackDefOf.NamerScenario);
			FilterMaker.filter.description = null;
			FilterMaker.filter.summary = null;

			Rand.PushState();
			Rand.Seed = seed.GetHashCode();
			FilterMaker.AddCategoryFilterParts(FilterMaker.filter, FilterPartCategory.Normal, Rand.RangeInclusive(3, 7));

			foreach (FilterPart.FilterPart part in FilterMaker.filter.AllParts) { part.Randomize(); }

			for (int i = 0; i < FilterMaker.filter.parts.Count; i++) {
				for (int j = 0; j < FilterMaker.filter.parts.Count; j++) {
					if (i != j && FilterMaker.filter.parts[i].TryMerge(FilterMaker.filter.parts[j])) {
						FilterMaker.filter.parts.RemoveAt(j);
						j--;
						if (i > j) { i--; }
					}
				}
			}

			for (int i = 0; i < FilterMaker.filter.parts.Count; i++) {
				for (int j = 0; j < FilterMaker.filter.parts.Count; j++) {
					if (i != j && !FilterMaker.filter.parts[i].CanCoexistWith(FilterMaker.filter.parts[j])) {
						FilterMaker.filter.parts.RemoveAt(j);
						j--;
						if (i > j) { i--; }
					}
				}
			}

			foreach (string item in FilterMaker.filter.ConfigErrors()) { Logger.LogErrorMessage(item); }

			Rand.PopState();

			Filter result = FilterMaker.filter;
			FilterMaker.filter = null;
			return result;
		}

		private static void AddCategoryFilterParts(Filter filter, FilterPartCategory category, int count) {
			filter.parts.AddRange(FilterMaker.RandomFilterPartsOfCategory(filter, category, count));
		}

		private static IEnumerable<FilterPart.FilterPart> RandomFilterPartsOfCategory(Filter filter, FilterPartCategory category, int count) {
			if (count <= 0) { yield break; }

			IEnumerable<FilterPartDef> allowedParts = from def in FilterMaker.AddableParts(filter) where def.category == category select def;
			int numYielded = 0;
			int numTries = 0;
			while (numYielded < count && allowedParts.Any()) {
				FilterPart.FilterPart part = FilterMaker.MakeFilterPart(allowedParts.RandomElementByWeight((FilterPartDef def) => def.selectionWeight));
				if (FilterMaker.CanAddPart(filter, part)) {
					yield return part;
					numYielded++;
				}
				numTries++;
				if (numTries > 100) {
					Logger.LogErrorMessage("Failed to add filter part.");
					break;
				}
			}
		}

		public static IEnumerable<FilterPartDef> AddableParts(Filter filter) {
			return DefDatabase<FilterPartDef>.AllDefs.Where((FilterPartDef def) => filter.AllParts.Count((FilterPart.FilterPart part) => part.def == def) < def.maxUses);
		}

		private static bool CanAddPart(Filter filter, FilterPart.FilterPart part) {
			for (int i = 0; i < filter.parts.Count; i++) {
				if (!part.CanCoexistWith(filter.parts[i])) { return false; }
			}
			return true;
		}

		public static FilterPart.FilterPart MakeFilterPart(FilterPartDef def) {
			FilterPart.FilterPart part = (FilterPart.FilterPart)Activator.CreateInstance(def.filterPartClass);
			part.def = def;
			return part;
		}
	}
}
