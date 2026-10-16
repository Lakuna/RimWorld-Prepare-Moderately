using System;
using System.IO;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Dialog_ScenarioList`.
	internal abstract class PawnFilterListDialog : Dialog_FileList {
		protected override void ReloadFiles() {
			this.files.Clear();
			foreach (FileInfo fileInfo in PawnFilter._allFiles) {
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					SaveFileInfo saveFileInfo = new SaveFileInfo(fileInfo);
#else
					SaveFileInfo saveFileInfo = new(fileInfo);
#endif
#if !(V1_0 || V1_1 || V1_2)
					saveFileInfo.LoadData();
#endif
					this.files.Add(saveFileInfo);
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					PrepareModeratelyLogger.LogException(e, "Failed to load filter.");
				}
			}
		}
	}
}
