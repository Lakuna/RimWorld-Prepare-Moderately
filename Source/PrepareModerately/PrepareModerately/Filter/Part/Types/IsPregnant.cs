using RimWorld;
using System;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsPregnant : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.def == HediffDefOf.Pregnant);

		public override string Summary(PawnFilter filter) => "IsPregnant".Translate();
	}
}
