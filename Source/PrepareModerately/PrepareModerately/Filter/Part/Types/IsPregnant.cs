#if !(V1_0 || V1_1 || V1_2 || V1_3)
using RimWorld;
using System;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsPregnant : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.def == HediffDefOf.PregnantHuman);

		public override string Summary(PawnFilter filter) => "IsPregnant".Translate();
	}
}
#endif
