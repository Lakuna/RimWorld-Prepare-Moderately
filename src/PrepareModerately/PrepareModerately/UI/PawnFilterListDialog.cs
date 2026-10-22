using System;
using System.IO;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Dialog_ScenarioList`.
	public abstract class PawnFilterListDialog : Dialog_FileList {
		protected override void ReloadFiles() {
			if (this.files is null) {
				PrepareModeratelyLogger.LogErrorMessage("Filter list dialog has null files.");
				return;
			}

			this.files.Clear();
			foreach (FileInfo fileInfo in PawnFilterFilePaths.All) {
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
				} catch (Exception e) {
					PrepareModeratelyLogger.LogException(e, "Failed to load filter.");
					throw;
				}
			}
		}
	}
}
