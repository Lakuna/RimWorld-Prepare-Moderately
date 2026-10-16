using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioLister`.
	internal static class PawnFilterLister {
		private static bool _dirty = true;

		internal static IEnumerable<PawnFilter> All() {
			RecacheIfDirty();
			foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs) {
				yield return def.filter;
			}

			foreach (PawnFilter filter in PawnFilter._localFilters) {
				yield return filter;
			}
		}

		internal static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			return DefDatabase<PawnFilterDef>.AllDefs.Any((def) => def.filter == filter)
				|| PawnFilter._localFilters.Any((localFilter) => localFilter == filter);
		}

		internal static IEnumerable<PawnFilter> InCategory(PawnFilterCategory category) {
			RecacheIfDirty();
			switch (category) {
				case PawnFilterCategory.FromDef:
					foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs) {
						yield return def.filter;
					}

					break;
				case PawnFilterCategory.CustomLocal:
					foreach (PawnFilter filter in PawnFilter._localFilters) {
						yield return filter;
					}

					break;
			}
		}

		internal static void MarkDirty() =>
			_dirty = true;

		private static int FilterListHash() {
			int hash = 9826121;
			foreach (PawnFilter filter in All()) {
				hash ^= 791 * filter.GetHashCode() * 6121;
			}

			return hash;
		}

		private static void Recache() {
			_dirty = false;
			int hash = FilterListHash();
			PawnFilter.RecacheLocalFilters();
			if (FilterListHash() != hash && !LongEventHandler.ShouldWaitForEvent) {
				Find.WindowStack.WindowOfType<SelectPawnFilterPage>()?.NotifyFilterListChanged();
			}
		}

		private static void RecacheIfDirty() {
			if (_dirty) {
				Recache();
			}
		}
	}
}
