using System;
using System.Collections.Generic;
using System.IO;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
using System.Linq;
#endif

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `Verse.GameDataSaveLoader`.
	internal static class PawnFilterSaveLoader {
		private const string FilterNodeName = "filter";

		private const string SavedFilterParentNodeName = "savedfilter";

		internal static bool Load(string absolutePath, PawnFilterCategory category, out PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			return LoadInternal(absolutePath, category, out filter);
#else
			using (Log.LockMessages()) {
				return LoadInternal(absolutePath, category, out filter);
			}
#endif
		}

		internal static void Save(PawnFilter filter, string absolutePath) {
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

		private static bool LoadInternal(string absolutePath, PawnFilterCategory category, out PawnFilter filter) {
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
				filter.Category = category;
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
			} catch (Exception e) {
#pragma warning restore CA1031
				PrepareModeratelyLogger.LogException(e, "Failed to load pawn filter.");
				filter = null;
				Scribe.ForceStop();
			}

			if (filter == null) {
				return false;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<PawnFilterPart> parts = filter.Parts.ToList();
#else
			List<PawnFilterPart> parts = [.. filter.Parts];
#endif
			if (parts.RemoveAll((part) => part.HasNullDefs()) > 0) {
				filter.Parts = parts;
				filter.Valid = false;
			}

			return true;
		}
	}
}
