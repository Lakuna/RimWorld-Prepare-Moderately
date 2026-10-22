using System;

#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

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
#if V1_0
			"PM.HasAnyPermanentMedicalConditions".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasAnyPermanentMedicalConditions".Translate().StringSafe();
#else
			"PM.HasAnyPermanentMedicalConditions".Translate().StringSafe;
#endif
	}
}
