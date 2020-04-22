using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NotHasRelationship : PawnFilterPart_HasRelationship {
		public PawnFilterPart_NotHasRelationship() : base() => this.label = "Doesn't have relationship:";

		public override bool Matches(Pawn pawn) => !base.Matches(pawn);
	}
}
