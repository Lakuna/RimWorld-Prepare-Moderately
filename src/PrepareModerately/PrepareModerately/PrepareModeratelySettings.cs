using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately {
	internal sealed class PrepareModeratelySettings : ModSettings {
		internal string FilterSavePath {
			get {
				if (this._filterSavePath.NullOrEmpty()) {
					this._filterSavePath = PawnFilter.DefaultDataPath;
				}

				return this._filterSavePath;
			}
			set => this._filterSavePath = value;
		}

		internal int RollSpeedMultiplier {
			get {
				if (this._rollSpeedMultiplier < 1) {
					this._rollSpeedMultiplier = 1;
				}

				return this._rollSpeedMultiplier;
			}
			set => this._rollSpeedMultiplier = value;
		}

		private string _filterSavePath;

		private int _rollSpeedMultiplier;

		public PrepareModeratelySettings() {
			this.FilterSavePath = PawnFilter.DefaultDataPath;
			this.RollSpeedMultiplier = 1;
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._filterSavePath, MiscUtil.NoUnder(nameof(this._filterSavePath)));
			Scribe_Values.Look(ref this._rollSpeedMultiplier, MiscUtil.NoUnder(nameof(this._rollSpeedMultiplier)));
		}
	}
}
