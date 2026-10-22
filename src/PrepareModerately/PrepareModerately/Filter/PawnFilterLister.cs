using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioLister`.
	public static class PawnFilterLister {
		public static IEnumerable<PawnFilter> All {
			get {
				RecacheIfDirty();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs ?? new List<PawnFilterDef>()) {
#else
				foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs ?? []) {
#endif
					if (def.Filter is null) {
						PrepareModeratelyLogger.LogErrorMessage("Attempted to cache null filter from definition.");
						continue;
					}

					yield return def.Filter;
				}

				foreach (PawnFilter filter in PawnFilterFiles.LocalFilters) {
					yield return filter;
				}
			}
		}

		private static bool _dirty = true;

		private static int _hash {
			get {
				int hash = 9826121;
				foreach (PawnFilter filter in All) {
					hash ^= 791 * filter.GetHashCode() * 6121;
				}

				return hash;
			}
		}

		public static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			return (DefDatabase<PawnFilterDef>.AllDefs?.Any((def) => def.Filter == filter) ?? false)
				|| PawnFilterFiles.LocalFilters.Any((localFilter) => localFilter == filter);
		}

		public static IEnumerable<PawnFilter> InCategory(PawnFilterCategory category) {
			RecacheIfDirty();
			switch (category) {
				case PawnFilterCategory.FromDef:
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs ?? new List<PawnFilterDef>()) {
#else
					foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefs ?? []) {
#endif
						if (def.Filter is null) {
							PrepareModeratelyLogger.LogErrorMessage("Attempted to list null filter from definition.");
							continue;
						}

						yield return def.Filter;
					}

					break;
				case PawnFilterCategory.CustomLocal:
					foreach (PawnFilter filter in PawnFilterFiles.LocalFilters) {
						yield return filter;
					}

					break;
			}
		}

		public static void MarkDirty() =>
			_dirty = true;

		private static void Recache() {
			_dirty = false;
			int hash = _hash;
			PawnFilterFiles.RecacheLocalFilters();
			if (_hash != hash && !LongEventHandler.ShouldWaitForEvent) {
				if (Find.WindowStack is null) {
					PrepareModeratelyLogger.LogErrorMessage("Attempted to find window in null stack.");
					return;
				}

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
