using PrepareModerately.UI;
using System.Linq;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasAnyAddiction : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn.health.hediffSet.hediffs.Any((hediff) => hediff.def.IsAddiction);

		public override void DoEditInterface(Listing_PawnFilter listing) => listing.GetPawnFilterPartRect(this, 0);
	}
}
