using System;

#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyAddiction : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff?.def?.IsAddiction ?? false) ?? false;

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasAnyAddiction".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasAnyAddiction".Translate().StringSafe();
#else
			"PM.HasAnyAddiction".Translate().StringSafe;
#endif
	}
}
