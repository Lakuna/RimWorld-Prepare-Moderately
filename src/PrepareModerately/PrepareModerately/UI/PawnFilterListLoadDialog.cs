using Lakuna.PrepareModerately.Filter;
using System;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	// Based on `RimWorld.Dialog_ScenarioList_Load`.
	public class PawnFilterListLoadDialog : PawnFilterListDialog {
		private readonly Action<PawnFilter> filterReturner;

		public PawnFilterListLoadDialog(Action<PawnFilter> filterReturner) {
			this.interactButLabel = "LoadGameButton".Translate().CapitalizeFirst();
			this.filterReturner = filterReturner;
		}

		protected override void DoFileInteraction(string fileName) {
			string filePath = PawnFilter.AbsolutePathForName(fileName);
			PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, () => {
				if (PawnFilterSaveLoader.Load(filePath, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					this.filterReturner(filter);
				}

				this.Close();
			});
		}
	}
}
