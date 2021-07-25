using PrepareModerately.UI;
using System.Linq;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasAnyBodyModification : PawnFilterPart {
		private static readonly ThingCategoryDef prostheticCategoryDef = DefDatabase<ThingCategoryDef>.GetNamed("BodyPartsProsthetic");

		public override bool Matches(Pawn pawn) => pawn.health.hediffSet.hediffs.Any((hediff) => hediff.source.thingCategories.Contains(prostheticCategoryDef));

		public override void DoEditInterface(Listing_PawnFilter listing) => listing.GetPawnFilterPartRect(this, 0);
	}
}
