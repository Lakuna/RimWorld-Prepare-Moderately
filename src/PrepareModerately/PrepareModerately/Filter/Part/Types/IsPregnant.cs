#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System;

using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class IsPregnant : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff.def == HediffDefOf.PregnantHuman) ?? false;

		public override string Summary(PawnFilter filter) =>
#if V1_4 || V1_5 || V1_6
			"PM.IsPregnant".Translate().StringSafe();
#else
			"PM.IsPregnant".Translate().StringSafe;
#endif
	}
}
#endif
