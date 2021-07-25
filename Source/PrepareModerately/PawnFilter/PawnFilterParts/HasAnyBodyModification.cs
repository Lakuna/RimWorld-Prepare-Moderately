using PrepareModerately.UI;
using System.Linq;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasAnyBodyModification : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn.def.race.body.AllParts.Any((bodyPart) => pawn.health.hediffSet.HasDirectlyAddedPartFor(bodyPart));

		public override void DoEditInterface(Listing_PawnFilter listing) => listing.GetPawnFilterPartRect(this, 0);
	}
}
