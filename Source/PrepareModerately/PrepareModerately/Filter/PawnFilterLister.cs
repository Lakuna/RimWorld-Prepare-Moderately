using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class PawnFilterLister {
		private static bool dirty = true;

		public static IEnumerable<PawnFilter> All() {
			RecacheIfDirty();
			foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefsListForReading) { yield return def.Filter; }
			foreach (PawnFilter filter in PawnFilter.LocalFilters) { yield return filter; }
		}

		public static IEnumerable<PawnFilter> InCategory(PawnFilterCategory category) {
			RecacheIfDirty();
			switch (category) {
				case PawnFilterCategory.FromDef:
					foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefsListForReading) { yield return def.Filter; }
					break;
				case PawnFilterCategory.CustomLocal:
					foreach (PawnFilter filter in PawnFilter.LocalFilters) { yield return filter; }
					break;
			}
		}

		public static bool FilterIsListedAnywhere(PawnFilter filter) {
			RecacheIfDirty();
			foreach (PawnFilterDef def in DefDatabase<PawnFilterDef>.AllDefsListForReading) {
				if (def.Filter == filter) { return true; }
			}
			foreach (PawnFilter localFilter in PawnFilter.LocalFilters) {
				if (localFilter == filter) { return true; }
			}
			return false;
		}

		public static void MarkDirty() {
			dirty = true;
		}

		private static void RecacheIfDirty() {
			if (dirty) { Recache(); }
		}

		private static void Recache() {
			dirty = false;
			int hash = FilterListHash();
			PawnFilter.RecacheLocalFiles();
			if (FilterListHash() != hash && !LongEventHandler.ShouldWaitForEvent) {
				// TODO: Find.WindowStack.WindowOfType<SelectFilterPage>()?.NotifyFilterListChanged();
			}
		}

		public static int FilterListHash() {
			int hash = 9081966;
			foreach (PawnFilter filter in All()) { hash ^= 313 * filter.GetHashCode() * 1228; }
			return hash;
		}
	}
}
