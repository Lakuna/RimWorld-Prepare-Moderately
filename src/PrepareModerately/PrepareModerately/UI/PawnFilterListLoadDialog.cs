using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Dialog_ScenarioList_Load`.
	public sealed class PawnFilterListLoadDialog : PawnFilterListDialog {
		private readonly Action<PawnFilter> _filterReturner;

		public PawnFilterListLoadDialog(Action<PawnFilter> filterReturner) {
			this._filterReturner = filterReturner;
#if V1_0
			this.interactButLabel = "PM.Load".Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.interactButLabel = "PM.Load".Translate().CapitalizeFirst().StringSafe();
#else
			this.interactButLabel = "PM.Load".Translate().CapitalizeFirst().StringSafe;
#endif
		}

		protected override void DoFileInteraction(string fileName) {
			string filePath = PawnFilterFilePaths.AbsolutePathForName(fileName);
			PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, () => {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				if (PawnFilterSaveLoader.Load(filePath, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
#else
				if (PawnFilterSaveLoader.Load(filePath, PawnFilterCategory.CustomLocal, out PawnFilter? filter)) {
#endif
					if (filter is null) {
						PrepareModeratelyLogger.LogErrorMessage("Attempted to load null filter.");
						return;
					}

					this._filterReturner(filter);
				}

				this.Close();
			});
		}
	}
}
