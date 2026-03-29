using System;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasAnyMissingBodyPart : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff is Hediff_MissingPart);

		public override string Summary(PawnFilter filter) => "PM.HasAnyMissingBodyPart".Translate();
	}
}
