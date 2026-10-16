using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioLister`.
	internal static class PawnFilterLister {
		private static bool s_dirty = true;

		internal static IEnumerable<PawnFilter> All() {
			RecacheIfDirty();
			foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs) {
				yield return def.PFilter;
			}

			foreach (PawnFilter filter in PawnFilter.LocalFilters) {
				yield return filter;
			}
		}

		internal static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			return DefDatabase<PawnFilterDef>.AllDefs.Any((def) => def.PFilter == filter)
				|| PawnFilter.LocalFilters.Any((localFilter) => localFilter == filter);
		}

		internal static IEnumerable<PawnFilter> InCategory(PawnFilterCategory category) {
			RecacheIfDirty();
			switch (category) {
				case PawnFilterCategory.FromDef:
					foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs) {
						yield return def.PFilter;
					}

					break;
				case PawnFilterCategory.CustomLocal:
					foreach (PawnFilter filter in PawnFilter.LocalFilters) {
						yield return filter;
					}

					break;
			}
		}

		internal static void MarkDirty() =>
			s_dirty = true;

		private static int FilterListHash() {
			int hash = 9826121;
			foreach (PawnFilter filter in All()) {
				hash ^= 791 * filter.GetHashCode() * 6121;
			}

			return hash;
		}

		private static void Recache() {
			s_dirty = false;
			int hash = FilterListHash();
			PawnFilter.RecacheLocalFilters();
			if (FilterListHash() != hash && !LongEventHandler.ShouldWaitForEvent) {
				Find.WindowStack.WindowOfType<SelectPawnFilterPage>()?.NotifyFilterListChanged();
			}
		}

		private static void RecacheIfDirty() {
			if (s_dirty) {
				Recache();
			}
		}
	}
}
