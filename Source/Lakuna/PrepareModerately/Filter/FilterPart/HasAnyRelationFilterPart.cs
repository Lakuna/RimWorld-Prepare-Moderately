using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAnyRelationFilterPart : FilterPart {
		public override bool Matches(Pawn pawn) {
			return pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
		}

		public override string Summary(Filter filter) {
			return "HasAnyRelation".Translate();
		}

		public override bool CanCoexistWith(FilterPart other) {
			return other.def != FilterPartDefOf.HasRelation;
		}
	}
}
