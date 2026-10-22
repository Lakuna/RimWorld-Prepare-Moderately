using System;

#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyScar : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff?.IsPermanent() ?? false) ?? false;

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasAnyScar".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasAnyScar".Translate().StringSafe();
#else
			"PM.HasAnyScar".Translate().StringSafe;
#endif
	}
}
