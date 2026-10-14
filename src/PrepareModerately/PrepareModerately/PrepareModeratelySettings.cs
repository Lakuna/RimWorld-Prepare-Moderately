using Lakuna.PrepareModerately.Filter;

using Verse;

namespace Lakuna.PrepareModerately {
	internal sealed class PrepareModeratelySettings : ModSettings {
		internal string PFilterSavePath {
			get {
				if (this.filterSavePath.NullOrEmpty()) {
					this.filterSavePath = PawnFilter.DefaultDataPath;
				}

				return this.filterSavePath;
			}
			set => this.filterSavePath = value;
		}

		internal int PRollSpeedMultiplier {
			get {
				if (this.rollSpeedMultiplier < 1) {
					this.rollSpeedMultiplier = 1;
				}

				return this.rollSpeedMultiplier;
			}
			set => this.rollSpeedMultiplier = value;
		}

		private string filterSavePath;

		private int rollSpeedMultiplier;

		public PrepareModeratelySettings() {
			this.PFilterSavePath = PawnFilter.DefaultDataPath;
			this.PRollSpeedMultiplier = 1;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.filterSavePath, nameof(this.filterSavePath));
			Scribe_Values.Look(ref this.rollSpeedMultiplier, nameof(this.rollSpeedMultiplier));
			base.ExposeData();
		}
	}
}
