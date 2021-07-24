using Verse;

namespace PrepareModerately {
	public class Settings : ModSettings {
		public int rollMultiplier;
		public int rollModulus;

		public Settings() {
			this.rollMultiplier = 1;
			this.rollModulus = 1;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.rollMultiplier, "rollMultiplier");
			Scribe_Values.Look(ref this.rollModulus, "rollModulus");

			base.ExposeData();
		}
	}
}
