using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `RimWorld.Dialog_ScenarioList_Save`.
	public sealed class PawnFilterListSaveDialog : PawnFilterListDialog {
		protected override bool ShouldDoTypeInField =>
			true;

		private readonly PawnFilter _filter;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private readonly Action _onClosed;
#else
		private readonly Action? _onClosed;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public PawnFilterListSaveDialog(PawnFilter filter, Action onClosed = null) {
#else
		public PawnFilterListSaveDialog(PawnFilter filter, Action? onClosed = null) {
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (filter is null) {
				throw new ArgumentNullException(nameof(filter));
			}
#else
			ArgumentNullException.ThrowIfNull(filter);
#endif

#if V1_0
			this.interactButLabel = "PM.Overwrite".Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.interactButLabel = "PM.Overwrite".Translate().CapitalizeFirst().StringSafe();
#else
			this.interactButLabel = "PM.Overwrite".Translate().CapitalizeFirst().StringSafe;
#endif
			this.typingName = filter.Name?.Trim() ?? "null";
			this._filter = filter;
			this._onClosed = onClosed;
		}

		public override void Close(bool doCloseSound = true) {
			base.Close(doCloseSound);
			this._onClosed?.Invoke();
		}

		protected override void DoFileInteraction(string fileName) {
			string sanitizedFileName = GenFile.SanitizedFileName(fileName) ?? fileName;
			LongEventHandler.QueueLongEvent(
				() => PawnFilterSaveLoader.Save(this._filter, PawnFilterFilePaths.AbsolutePathForName(sanitizedFileName)),
				"PM.Saving",
				false,
				(e) => PrepareModeratelyLogger.LogException(e, "Failed to save filter."));
			if (MessageTypeDefOf.SilentInput is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to use null message type definition for silent input.");
				return;
			}

#if V1_0
			Messages.Message("PM.SavedAsFile".Translate(sanitizedFileName)?.CapitalizeFirstSafe().EndWithPeriodSafe() ?? "null", MessageTypeDefOf.SilentInput, false);
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Messages.Message("PM.SavedAsFile".Translate(sanitizedFileName).CapitalizeFirst().EndWithPeriod().StringSafe(), MessageTypeDefOf.SilentInput, false);
#else
			Messages.Message("PM.SavedAsFile".Translate(sanitizedFileName).CapitalizeFirst().EndWithPeriod().StringSafe, MessageTypeDefOf.SilentInput, false);
#endif
			this.Close();
		}
	}
}
