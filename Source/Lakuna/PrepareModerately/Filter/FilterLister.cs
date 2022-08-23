using System.Collections.Generic;
using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class FilterLister {
		private static bool dirty;

		public static IEnumerable<Filter> AllFilters() {
			FilterLister.RecacheIfDirty();
			foreach (FilterDef def in DefDatabase<FilterDef>.AllDefs) { yield return def.filter; }
			foreach (Filter filter in FilterFiles.AllFiltersLocal) { yield return filter; }
		}

		public static IEnumerable<Filter> FiltersInCategory(FilterCategory category) {
			FilterLister.RecacheIfDirty();
			switch (category) {
				case FilterCategory.FromDef:
					foreach (FilterDef def in DefDatabase<FilterDef>.AllDefs) { yield return def.filter; }
					break;
				case FilterCategory.CustomLocal:
					foreach (Filter filter in FilterFiles.AllFiltersLocal) { yield return filter; }
					break;
			}
		}

		public static bool FilterIsListedAnywhere(Filter filter) {
			FilterLister.RecacheIfDirty();
			foreach (FilterDef def in DefDatabase<FilterDef>.AllDefs) {
				if (def.filter == filter) { return true; }
			}
			foreach (Filter localFilter in FilterFiles.AllFiltersLocal) {
				if (localFilter == filter) { return true; }
			}
			return false;
		}

		public static void MarkDirty() {
			FilterLister.dirty = true;
		}

		private static void RecacheIfDirty() {
			if (FilterLister.dirty) { FilterLister.RecacheData(); }
		}

		private static void RecacheData() {
			FilterLister.dirty = false;
			int hash = FilterLister.FilterListHash();
			FilterFiles.RecacheData();
			if (FilterLister.FilterListHash() != hash && !LongEventHandler.ShouldWaitForEvent) { Find.WindowStack.WindowOfType<SelectFilterPage>()?.NotifyFilterListChanged(); }
		}

		public static int FilterListHash() {
			int hash = 9081966;
			foreach (Filter filter in FilterLister.AllFilters()) { hash ^= 313 * filter.GetHashCode() * 1228; }
			return hash;
		}

		static FilterLister() {
			FilterLister.dirty = true;
		}
	}
}
