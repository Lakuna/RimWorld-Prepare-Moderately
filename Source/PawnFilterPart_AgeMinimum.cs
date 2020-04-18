using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_AgeMinimum : PawnFilterPart_AgeMaximum {
		public PawnFilterPart_AgeMinimum() {
			this.label = "Minimum age:";
			this.age = 20;
			this.buffer = "";
		}

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears >= this.age;
	}
}
