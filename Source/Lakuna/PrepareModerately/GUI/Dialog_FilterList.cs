using System;
using System.IO;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.GUI {
	// Based on RimWorld.Dialog_ScenarioList.
	public abstract class Dialog_FilterList : Dialog_FileList {
		protected override void ReloadFiles() {
			this.files.Clear();
			foreach (FileInfo allCustomFilterFile in PrepareModeratelyMod.AllCustomFilterFiles) {
				try {
					SaveFileInfo saveFileInfo = new SaveFileInfo(allCustomFilterFile);
					saveFileInfo.LoadData();
					this.files.Add(saveFileInfo);
				} catch (Exception e) {
					Logger.LogException(e, "exception loading " + allCustomFilterFile.Name);
				}
			}
		}
	}
}
