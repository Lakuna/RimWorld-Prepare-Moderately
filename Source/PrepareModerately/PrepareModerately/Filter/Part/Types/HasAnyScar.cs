using RimWorld;
using System;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasAnyScar : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((Hediff hediff) =>
				hediff.def == HediffDefOf.Bite
				|| hediff.def == HediffDefOf.Cut
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				|| hediff.def == HediffDefOf.Bruise
				|| hediff.def == HediffDefOf.Burn
				|| hediff.def == HediffDefOf.Gunshot
				|| hediff.def == HediffDefOf.Scratch
				|| hediff.def == HediffDefOf.Shredded
				|| hediff.def == HediffDefOf.Stab
#endif
#if !(V1_0 || V1_1 || V1_2)
				|| hediff.def == HediffDefOf.Scarification
#endif
			);

		public override string Summary(PawnFilter filter) => "HasAnyScar".Translate();
	}
}
