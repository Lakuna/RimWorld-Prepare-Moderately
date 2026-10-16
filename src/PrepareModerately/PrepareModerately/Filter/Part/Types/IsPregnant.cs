#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsPregnant : PawnFilterPart {
#pragma warning restore CA1812
		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff.def == HediffDefOf.PregnantHuman);

		internal override string Summary(PawnFilter filter) =>
			"PM.IsPregnant".Translate();
	}
}
#endif
