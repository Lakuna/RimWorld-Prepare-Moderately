using System.Collections.ObjectModel;
using System.IO;

using Lakuna.PrepareModerately.Utility;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioFiles`.
	public static class PawnFilterFiles {
		public static Collection<PawnFilter> LocalFilters =>
			_localFilters;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Collection<PawnFilter> _localFilters = new Collection<PawnFilter>();
#else
		private static readonly Collection<PawnFilter> _localFilters = [];
#endif

		public static void RecacheLocalFilters() {
			LocalFilters.Clear();
			foreach (FileInfo file in PawnFilterFilePaths.All) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
#else
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter? filter)) {
#endif
					if (filter is null) {
						PrepareModeratelyLogger.LogErrorMessage("Attempted to add null filter to list of local filters.");
						continue;
					}

					LocalFilters.Add(filter);
				}
			}
		}
	}
}
