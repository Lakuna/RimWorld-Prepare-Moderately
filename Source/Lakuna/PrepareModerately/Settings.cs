using Verse;

namespace Lakuna.PrepareModerately {
	public class Settings : ModSettings {
		public int pawnRollSpeedMultiplier;

		public Settings() {
			this.pawnRollSpeedMultiplier = 1;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.pawnRollSpeedMultiplier, nameof(this.pawnRollSpeedMultiplier));

			base.ExposeData();
		}
	}
}
