using Lakuna.PrepareModerately.UI;
using System.Collections.Generic;
using Verse;
using Verse.Noise;

namespace Lakuna.PrepareModerately.Filter {
	public static class PawnFilterLister {
		private static bool Dirty = true;

		public static IEnumerable<PawnFilter> All() {
			RecacheIfDirty();
			foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefsListForReading) { yield return def.filter; }
			foreach (PawnFilter filter in PawnFilter.LocalFilters) { yield return filter; }
		}

		public static IEnumerable<PawnFilter> InCategory(PawnFilterCategory category) {
			RecacheIfDirty();
			switch (category) {
				case PawnFilterCategory.FromDef:
					foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefsListForReading) {
						yield return def.filter;
					}
					break;
				case PawnFilterCategory.CustomLocal:
					foreach (PawnFilter filter in PawnFilter.LocalFilters) {
						yield return filter;
					}
					break;
			}
		}

		public static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefsListForReading) {
				if (def.filter == filter) { return true; }
			}
			foreach (PawnFilter localFilter in PawnFilter.LocalFilters) {
				if (localFilter == filter) { return true; }
			}
			return false;
		}

		public static void MarkDirty() => Dirty = true;

		private static void RecacheIfDirty() {
			if (Dirty) { Recache(); }
		}

		private static void Recache() {
			Dirty = false;
			int hash = FilterListHash();
			PawnFilter.RecacheLocalFilters();
			if (FilterListHash() != hash && !LongEventHandler.ShouldWaitForEvent) {
				Find.WindowStack.WindowOfType<SelectPawnFilterPage>()?.NotifyFilterListChanged();
			}
		}

		public static int FilterListHash() {
			int hash = 9081966;
			foreach (PawnFilter filter in All()) { hash ^= 313 * filter.GetHashCode() * 1228; }
			return hash;
		}
	}
}
