using System;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasAnyBodyModification : PawnFilterPart {
#pragma warning restore CA1812
		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.def.race.body.AllParts.Any(pawn.health.hediffSet.HasDirectlyAddedPartFor);

		internal override string Summary(PawnFilter filter) =>
			"PM.HasAnyBodyModification".Translate();
	}
}
