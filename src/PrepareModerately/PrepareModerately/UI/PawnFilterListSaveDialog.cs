using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Dialog_ScenarioList_Save`.
	internal sealed class PawnFilterListSaveDialog : PawnFilterListDialog {
		protected override bool ShouldDoTypeInField =>
			true;

		private readonly PawnFilter filter;

		internal PawnFilterListSaveDialog(PawnFilter filter) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

			this.interactButLabel = "PM.Overwrite".Translate().CapitalizeFirst();
			this.typingName = filter.PName;
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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			Messages.Message($"{"PM.SavedAsFile".Translate(fileName).CapitalizeFirst()}.", MessageTypeDefOf.SilentInput, false);
#else
			Messages.Message("PM.SavedAsFile".Translate(fileName).CapitalizeFirst().EndWithPeriod(), MessageTypeDefOf.SilentInput, false);
#endif
			this.Close();
		}
	}
}
