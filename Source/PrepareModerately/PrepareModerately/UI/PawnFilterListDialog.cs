using Lakuna.PrepareModerately.Filter;
using RimWorld;
using System;
using System.IO;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public abstract class PawnFilterListDialog : Dialog_FileList {
		protected override void ReloadFiles() {
			this.files.Clear();
			foreach (FileInfo fileInfo in PawnFilter.AllFiles) {
				try {
					files.Add(new SaveFileInfo(fileInfo));
					/*
					 * TODO (1.?+):
					 * SaveFileInfo saveFileInfo = new SaveFileInfo(fileInfo);
					 * saveFileinfo.LoadData();
					 * this.files.Add(saveFileInfo);
					 */
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					Logger.LogException(e, "Failed to load filter.");
				}
			}
		}
	}
}
