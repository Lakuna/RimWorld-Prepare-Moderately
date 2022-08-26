using System;
using System.IO;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class FilterSaveLoader {
		public const string SavedFilterParentNodeName = "savedfilter";

		public const string FilterNodeName = "filter";

		public static void SaveFilter(Filter filter, string absoluteFilePath) {
			try {
				filter.fileName = Path.GetFileNameWithoutExtension(absoluteFilePath);
				SafeSaver.Save(absoluteFilePath, FilterSaveLoader.SavedFilterParentNodeName, delegate {
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Scribe_Deep.Look(ref filter, FilterSaveLoader.FilterNodeName);
				});
			} catch (Exception e) {
				Logger.LogException(e, "Failed to save filter.");
			}
		}

		public static bool TryLoadFilter(string absoluteFilePath, FilterCategory category, out Filter filter) {
			filter = null;
			try {
				Scribe.loader.InitLoading(absoluteFilePath);
				try {
					ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, false); // Using the scenario scribe header mode works fine.
					Scribe_Deep.Look(ref filter, FilterSaveLoader.FilterNodeName);
					Scribe.loader.FinalizeLoading();
				} catch {
					Scribe.ForceStop();
					throw;
				}
				filter.fileName = Path.GetFileNameWithoutExtension(new FileInfo(absoluteFilePath).Name);
				filter.Category = category;
			} catch (Exception e) {
				Logger.LogException(e, "Failed to load filter.");
				filter = null;
				Scribe.ForceStop();
			}
			return filter != null;
		}
	}
}
