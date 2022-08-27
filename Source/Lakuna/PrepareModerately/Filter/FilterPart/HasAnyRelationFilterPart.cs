using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAnyRelationFilterPart : FilterPart {
		public override bool Matches(Pawn pawn) {
			return pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			listing.GetFilterPartRect(this, 0);
		}

		public override string Summary(Filter filter) {
			return "Has any relationship.";
		}

		public override bool CanCoexistWith(FilterPart other) {
			return other.def != FilterPartDefOf.HasRelation;
		}
	}
}
