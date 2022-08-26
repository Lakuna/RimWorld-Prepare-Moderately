using Lakuna.PrepareModerately.Filter;
using Verse;

namespace Lakuna.PrepareModerately {
	public class Settings : ModSettings {
		public string filterSavePath;

		public Settings() {
			this.filterSavePath = GenFilterPaths.FolderUnderSaveData("Filter");
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.filterSavePath, nameof(this.filterSavePath));

			base.ExposeData();
		}
	}
}
