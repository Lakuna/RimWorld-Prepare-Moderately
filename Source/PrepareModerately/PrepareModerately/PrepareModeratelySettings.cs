using Lakuna.PrepareModerately.Filter;
using Verse;

namespace Lakuna.PrepareModerately {
	public class PrepareModeratelySettings : ModSettings {
		private string filterSavePath;

		public string FilterSavePath {
			get => this.filterSavePath;
			set => this.filterSavePath = value;
		}

		public PrepareModeratelySettings() => this.filterSavePath = PawnFilter.DefaultPawnFiltersFolderPath;

		public override void ExposeData() {
			Scribe_Values.Look(ref this.filterSavePath, nameof(this.filterSavePath));
			base.ExposeData();
		}
	}
}
