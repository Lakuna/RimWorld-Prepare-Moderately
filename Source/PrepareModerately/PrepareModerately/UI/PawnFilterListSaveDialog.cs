using Lakuna.PrepareModerately.Filter;
using RimWorld;
using System;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class PawnFilterListSaveDialog : PawnFilterListDialog {
		private readonly PawnFilter filter;

		protected override bool ShouldDoTypeInField => true;

		public PawnFilterListSaveDialog(PawnFilter filter) {
			if (filter == null) {
				throw new ArgumentNullException(nameof(filter));
			}

			this.interactButLabel = "OverwriteButton".Translate().CapitalizeFirst();
			this.typingName = filter.Name;
			this.filter = filter;
		}

		protected override void DoFileInteraction(string fileName) {
			fileName = GenFile.SanitizedFileName(fileName);
			string absolutePath = PawnFilter.AbsolutePathForName(fileName);
			LongEventHandler.QueueLongEvent(delegate {
				PawnFilterSaveLoader.Save(this.filter, absolutePath);
			}, "SavingLongEvent", false, delegate (Exception e) {
				Logger.LogException(e, "Failed to save filter.");
			});
			Messages.Message("SavedAs".Translate(fileName).CapitalizeFirst(), MessageTypeDefOf.SilentInput, false);
			this.Close();
		}
	}
}
