using Lakuna.PrepareModerately.Filter;

using Verse;

namespace Lakuna.PrepareModerately {
	internal sealed class PrepareModeratelySettings : ModSettings {
#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal string filterSavePath;

		internal int rollSpeedMultiplier;
#pragma warning restore CA1051, IDE1006

		public PrepareModeratelySettings() {
			this.filterSavePath = PawnFilter.DefaultDataPath;
			this.rollSpeedMultiplier = 1;
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.filterSavePath, nameof(this.filterSavePath));
			Scribe_Values.Look(ref this.rollSpeedMultiplier, nameof(this.rollSpeedMultiplier));
		}
	}
}
