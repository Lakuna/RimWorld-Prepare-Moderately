#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsPregnant : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff.def == HediffDefOf.PregnantHuman);

		public override string Summary(PawnFilter filter) => "PM.IsPregnant".Translate();
	}
}
#endif
