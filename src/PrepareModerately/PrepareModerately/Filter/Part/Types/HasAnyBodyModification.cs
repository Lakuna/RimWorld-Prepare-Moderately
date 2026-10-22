using System;

#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyBodyModification : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			pawn.health.hediffSet != null
#else
			pawn.health.hediffSet is not null
#endif
			&& (pawn.def?.race?.body?.AllParts?.Any(pawn.health.hediffSet.HasDirectlyAddedPartFor) ?? false);

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasAnyBodyModification".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasAnyBodyModification".Translate().StringSafe();
#else
			"PM.HasAnyBodyModification".Translate().StringSafe;
#endif
	}
}
