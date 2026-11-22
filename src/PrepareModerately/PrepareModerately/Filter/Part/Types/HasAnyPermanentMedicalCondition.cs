using System;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyPermanentMedicalCondition : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) =>
				(hediff?.IsPermanent() ?? false)
#if !V1_0
				|| (hediff?.def?.chronic ?? false)
#endif
			) ?? false;

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyPermanentMedicalConditions".TranslateFull();
	}
}
