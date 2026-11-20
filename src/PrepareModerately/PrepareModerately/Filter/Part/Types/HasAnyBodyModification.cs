using System;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyBodyModification : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(pawn.health.hediffSet is null)
#else
			pawn.health.hediffSet is not null
#endif
			&& (pawn.def?.race?.body?.AllParts?.Any(pawn.health.hediffSet.HasDirectlyAddedPartFor) ?? false);

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyBodyModification".TranslateFull();
	}
}
