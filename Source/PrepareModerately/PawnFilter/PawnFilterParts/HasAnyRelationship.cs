using PrepareModerately.UI;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasAnyRelationship : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;

		public override void DoEditInterface(Listing_PawnFilter listing) => listing.GetPawnFilterPartRect(this, 0);
	}
}
