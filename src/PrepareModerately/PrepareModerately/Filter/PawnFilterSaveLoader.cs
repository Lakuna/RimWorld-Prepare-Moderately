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
		private const string _filterNodeName = "filter";

		private const string _savedFilterParentNodeName = "savedfilter";

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
				filter._fileName = Path.GetFileNameWithoutExtension(absolutePath);
				SafeSaver.Save(absolutePath, _savedFilterParentNodeName, () => {
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Scribe_Deep.Look(ref filter, _filterNodeName);
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
					Scribe_Deep.Look(ref filter, _filterNodeName);
					Scribe.loader.FinalizeLoading();
				} catch {
					Scribe.ForceStop();
					throw;
				}

				filter._fileName = Path.GetFileNameWithoutExtension(new FileInfo(absolutePath).Name);
				filter._category = category;
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
			List<PawnFilterPart> parts = filter.parts.ToList();
#else
			List<PawnFilterPart> parts = [.. filter.parts];
#endif
			if (parts.RemoveAll((part) => part.HasNullDefs()) > 0) {
				filter.parts = parts;
				filter._valid = false;
			}

			return true;
		}
	}
}
