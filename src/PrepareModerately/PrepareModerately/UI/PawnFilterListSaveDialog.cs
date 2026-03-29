using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.UI {
	// Based on `RimWorld.Dialog_ScenarioList_Save`.
	public class PawnFilterListSaveDialog : PawnFilterListDialog {
		private readonly PawnFilter filter;

		protected override bool ShouldDoTypeInField => true;

		public PawnFilterListSaveDialog(PawnFilter filter) {
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}

			this.interactButLabel = "PM.Overwrite".Translate().CapitalizeFirst();
			this.typingName = filter.Name;
			this.filter = filter;
		}

		protected override void DoFileInteraction(string fileName) {
			fileName = GenFile.SanitizedFileName(fileName);
			string absolutePath = PawnFilter.AbsolutePathForName(fileName);
			LongEventHandler.QueueLongEvent(
				() => PawnFilterSaveLoader.Save(this.filter, absolutePath),
				"SavingLongEvent",
				false,
				(e) => PrepareModeratelyLogger.LogException(e, "Failed to save filter."));
			Messages.Message(MiscellaneousUtility.EndWithPeriod("PM.SavedAsFile".Translate(fileName).CapitalizeFirst()), MessageTypeDefOf.SilentInput, false);
			this.Close();
		}
	}
}
