using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAnyBodyModificationFilterPart : FilterPart {
		public override bool Matches(Pawn pawn) {
			return pawn.def.race.body.AllParts.Any((BodyPartRecord bodyPart) => pawn.health.hediffSet.HasDirectlyAddedPartFor(bodyPart));
		}

		public override void DoEditInterface(FilterEditListing listing) {
			listing.GetFilterPartRect(this, 0);
		}

		public override string Summary(Filter filter) {
			return "Has any body modification.";
		}
	}
}
