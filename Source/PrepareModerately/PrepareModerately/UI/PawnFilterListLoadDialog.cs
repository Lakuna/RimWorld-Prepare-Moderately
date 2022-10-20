using Lakuna.PrepareModerately.Filter;
using System;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class PawnFilterListLoadDialog : PawnFilterListDialog {
		private readonly Action<PawnFilter> filterReturner;

		public PawnFilterListLoadDialog(Action<PawnFilter> filterReturner) {
			this.interactButLabel = "LoadGameButton".Translate().CapitalizeFirst();
			this.filterReturner = filterReturner;
		}

		protected override void DoFileInteraction(string fileName) {
			string filePath = PawnFilter.AbsolutePathForName(fileName);
			PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, delegate {
				if (PawnFilterSaveLoader.Load(filePath, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					filterReturner(filter);
				}
				this.Close();
			});
		}
	}
}
