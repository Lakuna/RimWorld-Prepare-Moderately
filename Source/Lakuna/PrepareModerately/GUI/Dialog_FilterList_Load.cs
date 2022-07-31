using System;
using Lakuna.PrepareModerately.PawnFilter;
using Verse;

namespace Lakuna.PrepareModerately.GUI {
	// Based on RimWorld.Dialog_ScenarioList_Load.
	public class Dialog_FilterList_Load : Dialog_FilterList {
		private Action<Filter> filterReturner;

		public Dialog_FilterList_Load(Action<Filter> filterReturner) {
			this.interactButLabel = "LoadGameButton".Translate();
			this.filterReturner = filterReturner;
		}

		protected override void DoFileInteraction(string fileName) {
			string filePath = PrepareModeratelyMod.AbsolutePathForFilter(fileName);
			// WIP
		}
	}
}
