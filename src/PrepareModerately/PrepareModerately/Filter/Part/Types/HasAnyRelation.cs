using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasAnyRelation : PawnFilterPart {
		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.HasRelation;

		public override bool Matches(Pawn pawn) =>
			pawn.relations?.RelatedToAnyoneOrAnyoneRelatedToMe ?? false;

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyRelation".TranslateFull();
	}
}
