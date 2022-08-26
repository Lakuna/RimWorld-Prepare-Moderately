using System;
using System.IO;
using Lakuna.PrepareModerately.Filter;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public abstract class FilterListDialog : Dialog_FileList {
		protected override void ReloadFiles() {
			this.files.Clear();
			foreach (FileInfo customFilterFile in GenFilterPaths.AllCustomFilterFiles) {
				try {
					SaveFileInfo saveFileInfo = new SaveFileInfo(customFilterFile);
					saveFileInfo.LoadData();
					this.files.Add(saveFileInfo);
				} catch (Exception e) {
					Logger.LogException(e, "Exception loading custom filter file.");
				}
			}
		}
	}
}
