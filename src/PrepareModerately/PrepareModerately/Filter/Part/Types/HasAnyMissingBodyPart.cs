using System;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasAnyMissingBodyPart : PawnFilterPart {
#pragma warning restore CA1812
		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.health.hediffSet.hediffs.Any((hediff) => hediff is Hediff_MissingPart);

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyMissingBodyPart".Translate();
	}
}
