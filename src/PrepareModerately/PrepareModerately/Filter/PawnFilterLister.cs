using System.Collections.Generic;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioLister`.
	internal static class PawnFilterLister {
		private static bool Dirty = true;

		private static IEnumerable<PawnFilterDef> LegalPawnFilters =>
			DefDatabase<PawnFilterDef>.AllDefs;

		internal static IEnumerable<PawnFilter> All() {
			RecacheIfDirty();
			foreach (PawnFilterDef def in LegalPawnFilters) {
				yield return def.PFilter;
			}

			foreach (PawnFilter filter in PawnFilter.LocalFilters) {
				yield return filter;
			}
		}

		internal static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			foreach (PawnFilterDef def in LegalPawnFilters) {
				if (def.PFilter == filter) {
					return true;
				}
			}

			foreach (PawnFilter localFilter in PawnFilter.LocalFilters) {
				if (localFilter == filter) {
					return true;
				}
			}

			return false;
		}

		internal static IEnumerable<PawnFilter> InCategory(PawnFilterCategory category) {
			RecacheIfDirty();
			switch (category) {
				case PawnFilterCategory.FromDef:
					foreach (PawnFilterDef def in LegalPawnFilters) {
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
			Dirty = true;

		private static int FilterListHash() {
			int hash = 9826121;
			foreach (PawnFilter filter in All()) {
				hash ^= 791 * filter.GetHashCode() * 6121;
			}

			return hash;
		}

		private static void Recache() {
			Dirty = false;
			int hash = FilterListHash();
			PawnFilter.RecacheLocalFilters();
			if (FilterListHash() != hash && !LongEventHandler.ShouldWaitForEvent) {
				Find.WindowStack.WindowOfType<SelectPawnFilterPage>()?.notifyFilterListChanged();
			}
		}

		private static void RecacheIfDirty() {
			if (Dirty) {
				Recache();
			}
		}
	}
}
