using System;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasAnyPermanentMedicalCondition : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff.IsPermanent());
#else
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff.IsPermanent() || hediff.def.chronic);
#endif

		public override string Summary(PawnFilter filter) => "HasAnyPermanentMedicalConditions".Translate();
	}
}
