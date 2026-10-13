using System;
using System.IO;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `Verse.GameDataSaveLoader`.
	public static class PawnFilterSaveLoader {
		private const string SavedFilterParentNodeName = "savedfilter";

		private const string FilterNodeName = "filter";

		public static void Save(PawnFilter filter, string absolutePath) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			try {
				filter.FileName = Path.GetFileNameWithoutExtension(absolutePath);
				SafeSaver.Save(absolutePath, SavedFilterParentNodeName, () => {
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Scribe_Deep.Look(ref filter, FilterNodeName);
				});
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
			} catch (Exception e) {
#pragma warning restore CA1031
				PrepareModeratelyLogger.LogException(e, "Failed to save pawn filter.");
			}
		}

		public static bool Load(string absolutePath, PawnFilterCategory category, out PawnFilter filter) {
			filter = null;

			try {
				Scribe.loader.InitLoading(absolutePath);
				try {
					// Using the scenario scribe header mode works fine.
					ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, false);
					Scribe_Deep.Look(ref filter, FilterNodeName);
					Scribe.loader.FinalizeLoading();
				} catch {
					Scribe.ForceStop();
					throw;
				}

				filter.FileName = Path.GetFileNameWithoutExtension(new FileInfo(absolutePath).Name);
				filter.CategoryP = category;
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
			} catch (Exception e) {
#pragma warning restore CA1031
				PrepareModeratelyLogger.LogException(e, "Failed to load pawn filter.");
				filter = null;
				Scribe.ForceStop();
			}

			return filter != null;
		}
	}
}
