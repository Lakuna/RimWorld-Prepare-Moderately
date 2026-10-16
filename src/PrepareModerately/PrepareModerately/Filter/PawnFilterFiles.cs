using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioFiles`.
	internal static class PawnFilterFiles {
		internal static IEnumerable<PawnFilter> _localFilters =>
			_localFiltersInternal;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Collection<PawnFilter> _localFiltersInternal = new Collection<PawnFilter>();
#else
		private static readonly Collection<PawnFilter> _localFiltersInternal = [];
#endif

		internal static void RecacheLocalFilters() {
			_localFiltersInternal.Clear();
			foreach (FileInfo file in PawnFilterFilePaths._allFiles) {
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					_localFiltersInternal.Add(filter);
				}
			}
		}
	}
}
