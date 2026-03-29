using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using System.IO;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	// Based on `RimWorld.Dialog_ScenarioList`.
	public abstract class PawnFilterListDialog : Dialog_FileList {
		protected override void ReloadFiles() {
			this.files.Clear();
			foreach (FileInfo fileInfo in PawnFilter.AllFiles) {
				try {
					SaveFileInfo saveFileinfo = new SaveFileInfo(fileInfo);
#if V1_0 || V1_1 || V1_2
					this.files.Add(saveFileinfo);
#else
					saveFileinfo.LoadData();
					this.files.Add(saveFileinfo);
#endif
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					PrepareModeratelyLogger.LogException(e, "Failed to load filter.");
				}
			}
		}
	}
}
