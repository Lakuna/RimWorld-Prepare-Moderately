using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioLister`.
	internal static class PawnFilterLister {
		internal static IEnumerable<PawnFilter> _all {
			get {
				RecacheIfDirty();
				foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs) {
					yield return def.filter;
				}

				foreach (PawnFilter filter in PawnFilterFiles._localFilters) {
					yield return filter;
				}
			}
		}

		private static bool _dirty = true;

		private static int _hash {
			get {
				int hash = 9826121;
				foreach (PawnFilter filter in _all) {
					hash ^= 791 * filter.GetHashCode() * 6121;
				}

				return hash;
			}
		}

		internal static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			return DefDatabase<PawnFilterDef>.AllDefs.Any((def) => def.filter == filter)
				|| PawnFilterFiles._localFilters.Any((localFilter) => localFilter == filter);
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
					foreach (PawnFilter filter in PawnFilterFiles._localFilters) {
						yield return filter;
					}

					break;
			}
		}

		internal static void MarkDirty() =>
			_dirty = true;

		private static void Recache() {
			_dirty = false;
			int hash = _hash;
			PawnFilterFiles.RecacheLocalFilters();
			if (_hash != hash && !LongEventHandler.ShouldWaitForEvent) {
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
