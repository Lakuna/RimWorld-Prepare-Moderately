using System;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasAnyScar : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.IsPermanent());

		public override string Summary(PawnFilter filter) => "HasAnyScar".Translate();
	}
}
