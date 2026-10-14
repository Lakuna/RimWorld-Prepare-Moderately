using System;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasAnyPermanentMedicalCondition : PawnFilterPart {
#pragma warning restore CA1812
		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff.IsPermanent());
#else
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff.IsPermanent() || hediff.def.chronic);
#endif

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyPermanentMedicalConditions".Translate();
	}
}
