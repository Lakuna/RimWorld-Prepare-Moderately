using Lakuna.PrepareModerately.Utility;
using System;
using System.IO;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class PawnFilterSaveLoader {
		private const string savedFilterParentNodeName = "savedfilter";

		private const string filterNodeName = "filter";

		public static void Save(PawnFilter filter, string absolutePath) {
			if (filter == null) {
				throw new ArgumentNullException(nameof(filter));
			}

			try {
				filter.FileName = Path.GetFileNameWithoutExtension(absolutePath);
				SafeSaver.Save(absolutePath, savedFilterParentNodeName, delegate {
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Scribe_Deep.Look(ref filter, filterNodeName);
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
					Scribe_Deep.Look(ref filter, filterNodeName);
					Scribe.loader.FinalizeLoading();
				} catch {
					Scribe.ForceStop();
					throw;
				}
				filter.FileName = Path.GetFileNameWithoutExtension(new FileInfo(absolutePath).Name);
				filter.Category = category;
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
