using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_DisallowedTrait : PawnFilterPart_RequiredTrait {
		public PawnFilterPart_DisallowedTrait() : base() => this.label = "Disallowed trait:";

		public override bool Matches(Pawn pawn) => !base.Matches(pawn);
	}
}
