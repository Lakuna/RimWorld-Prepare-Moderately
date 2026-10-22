#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasAnyRelation : PawnFilterPart {
		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.HasRelation;

		public override bool Matches(Pawn pawn) =>
			pawn.relations?.RelatedToAnyoneOrAnyoneRelatedToMe ?? false;

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasAnyRelation".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasAnyRelation".Translate().StringSafe();
#else
			"PM.HasAnyRelation".Translate().StringSafe;
#endif
	}
}
