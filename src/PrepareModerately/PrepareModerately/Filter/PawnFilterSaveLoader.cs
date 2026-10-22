using System;
using System.IO;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `Verse.GameDataSaveLoader`.
	public static class PawnFilterSaveLoader {
		private const string _filterNodeName = "filter";

		private const string _savedFilterParentNodeName = "savedfilter";

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool Load(string absolutePath, PawnFilterCategory category, out PawnFilter filter) {
#else
		public static bool Load(string absolutePath, PawnFilterCategory category, out PawnFilter? filter) {
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			return LoadInternal(absolutePath, category, out filter);
#else
			using (Log.LockMessages()) {
				return LoadInternal(absolutePath, category, out filter);
			}
#endif
		}

		public static void Save(PawnFilter filter, string absolutePath) {
			if (filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to save null filter.");
				return;
			}

			try {
				filter.FileName = Path.GetFileNameWithoutExtension(absolutePath);
				SafeSaver.Save(absolutePath, _savedFilterParentNodeName, () => {
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Scribe_Deep.Look(ref filter, _filterNodeName);
				});
			} catch (Exception e) {
				PrepareModeratelyLogger.LogException(e, "Failed to save pawn filter.");
				return;
				throw;
			}
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static bool LoadInternal(string absolutePath, PawnFilterCategory category, out PawnFilter filter) {
#else
		private static bool LoadInternal(string absolutePath, PawnFilterCategory category, out PawnFilter? filter) {
#endif
			filter = null;

			if (Scribe.loader is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to load filter with null scribe loader.");
				return false;
			}

			try {
				Scribe.loader.InitLoading(absolutePath);
				try {
					ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, false); // Using the scenario scribe header mode works fine.
					Scribe_Deep.Look(ref filter, _filterNodeName);
					Scribe.loader.FinalizeLoading();
				} catch (Exception e) {
					PrepareModeratelyLogger.LogException(e, "Error while loading filter.");
					Scribe.ForceStop();
					throw;
				}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				if (filter != null) {
#else
				if (filter is not null) {
#endif
					filter.FileName = Path.GetFileNameWithoutExtension(new FileInfo(absolutePath).Name);
					filter.Category = category;
				}
			} catch (Exception e) {
				PrepareModeratelyLogger.LogException(e, "Failed to load pawn filter.");
				filter = null;
				Scribe.ForceStop();
				throw;
			}

			if (filter is null) {
				return false;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter.Parts != null) {
#else
			if (filter.Parts is not null) {
#endif
				for (int i = filter.Parts.Count - 1; i >= 0; i--) {
					if (filter.Parts[i].Valid) {
						continue;
					}

					filter.Parts.RemoveAt(i);
					filter.Valid = false;
				}
			}

			return true;
		}
	}
}
