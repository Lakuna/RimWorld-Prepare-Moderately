using Lakuna.PrepareModerately.Filter;
using Verse;

namespace Lakuna.PrepareModerately {
	public class PrepareModeratelySettings : ModSettings {
		private string filterSavePath;

		private int rollSpeedMultiplier;

		public string FilterSavePath {
			get {
				if (string.IsNullOrEmpty(this.filterSavePath)) {
					this.filterSavePath = PawnFilter.DefaultDataPath;
				}

				return this.filterSavePath;
			}
			set => this.filterSavePath = value;
		}

		public int RollSpeedMultiplier {
			get {
				if (this.rollSpeedMultiplier < 1) {
					this.rollSpeedMultiplier = 1;
				}

				return this.rollSpeedMultiplier;
			}
			set => this.rollSpeedMultiplier = value;
		}

		public PrepareModeratelySettings() {
			this.FilterSavePath = PawnFilter.DefaultDataPath;
			this.RollSpeedMultiplier = 1;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.filterSavePath, nameof(this.filterSavePath));
			Scribe_Values.Look(ref this.rollSpeedMultiplier, nameof(this.rollSpeedMultiplier));
			base.ExposeData();
		}
	}
}
