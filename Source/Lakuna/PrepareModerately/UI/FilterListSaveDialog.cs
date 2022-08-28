using System;
using Lakuna.PrepareModerately.Filter;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class FilterListSaveDialog : FilterListDialog {
		private Filter.Filter savingFilter;

		protected override bool ShouldDoTypeInField => true;

		public FilterListSaveDialog(Filter.Filter filter) {
			this.interactButLabel = "OverwriteButton".Translate().CapitalizeFirst();
			this.typingName = filter.name;
			this.savingFilter = filter;
		}

		protected override void DoFileInteraction(string fileName) {
			fileName = GenFile.SanitizedFileName(fileName);
			string absolutePath = GenFilterPaths.AbsolutePathForFilter(fileName);
			LongEventHandler.QueueLongEvent(delegate {
				FilterSaveLoader.SaveFilter(this.savingFilter, absolutePath);
			}, "SavingLongEvent", false, delegate (Exception e) {
				Logger.LogException(e, "FailedToSaveFilter".Translate());
			});
			Messages.Message("SavedAs".Translate(fileName).CapitalizeFirst(), MessageTypeDefOf.SilentInput, false);
			this.Close();
		}
	}
}
