using System;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyPermanentMedicalCondition : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
#if V1_0
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff?.IsPermanent() ?? false) ?? false;
#else
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => (hediff?.IsPermanent() ?? false) || (hediff?.def?.chronic ?? false)) ?? false;
#endif

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyPermanentMedicalConditions".TranslateFull();
	}
}
