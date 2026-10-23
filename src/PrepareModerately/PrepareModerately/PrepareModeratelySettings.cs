using Lakuna.PrepareModerately.Filter;

using Verse;

namespace Lakuna.PrepareModerately {
	public sealed class PrepareModeratelySettings : ModSettings {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public string FilterSavePath {
#else
		public string? FilterSavePath {
#endif
			get => this._filterSavePath;
			set => this._filterSavePath = value;
		}

		public bool HighVisibilityMode {
			get => this._highVisibilityMode;
			set => this._highVisibilityMode = value;
		}

		public float RollSpeedMultiplier {
			get => this._rollSpeedMultiplier;
			set => this._rollSpeedMultiplier = value;
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private string _filterSavePath = PawnFilterFilePaths.DefaultDataPath;
#else
		private string? _filterSavePath = PawnFilterFilePaths.DefaultDataPath;
#endif

		private bool _highVisibilityMode;

		private float _rollSpeedMultiplier = 1;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._filterSavePath, "filterSavePath");
			Scribe_Values.Look(ref this._highVisibilityMode, "highVisibilityMode");
			Scribe_Values.Look(ref this._rollSpeedMultiplier, "rollSpeedMultiplier");
		}
	}
}
