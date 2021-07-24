using PrepareModerately.UI;
using System.Linq;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class IsCapableOfEverything : PawnFilterPart {
		public override bool Matches(Pawn pawn) => DefDatabase<WorkTypeDef>.AllDefsListForReading.All((def) => !pawn.WorkTypeIsDisabled(def));

		public override void DoEditInterface(Listing_PawnFilter listing) => listing.GetPawnFilterPartRect(this, 0);
	}
}
