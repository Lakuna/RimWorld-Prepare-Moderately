using System;

using Lakuna.PrepareModerately.Filter;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Dialog_ScenarioList_Load`.
	internal sealed class PawnFilterListLoadDialog : PawnFilterListDialog {
		private readonly Action<PawnFilter> _filterReturner;

		internal PawnFilterListLoadDialog(Action<PawnFilter> filterReturner) {
			this.interactButLabel = "PM.Load".Translate().CapitalizeFirst();
			this._filterReturner = filterReturner;
		}

		protected override void DoFileInteraction(string fileName) {
			string filePath = PawnFilterFilePaths.AbsolutePathForName(fileName);
			PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, () => {
				if (PawnFilterSaveLoader.Load(filePath, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					this._filterReturner(filter);
				}

				this.Close();
			});
		}
	}
}
