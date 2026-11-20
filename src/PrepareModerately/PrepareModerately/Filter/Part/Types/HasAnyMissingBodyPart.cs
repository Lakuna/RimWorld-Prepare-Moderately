using System;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyMissingBodyPart : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff is Hediff_MissingPart) ?? false;

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyMissingBodyPart".TranslateFull();
	}
}
