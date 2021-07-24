using PrepareModerately.UI;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class NameContains : PawnFilterPart {
		public string segment;

		public NameContains() => this.segment = "Tynan";

		public override bool Matches(Pawn pawn) => pawn.Name.ToStringFull.Contains(this.segment);

		public override void DoEditInterface(Listing_PawnFilter listing) => this.segment = Widgets.TextArea(listing.GetPawnFilterPartRect(this, Text.LineHeight), this.segment);
	}
}
