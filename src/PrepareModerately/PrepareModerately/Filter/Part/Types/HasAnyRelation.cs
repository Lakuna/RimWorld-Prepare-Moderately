using System;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasAnyRelation : PawnFilterPart {
#pragma warning restore CA1812
		internal override bool CanCoexistWith(PawnFilterPart other) =>
			other is null ? throw new ArgumentNullException(nameof(other))
			: other.def != PawnFilterPartDefOf.HasRelation;

		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;

		internal override string Summary(PawnFilter filter) =>
			"PM.HasAnyRelation".Translate();
	}
}
