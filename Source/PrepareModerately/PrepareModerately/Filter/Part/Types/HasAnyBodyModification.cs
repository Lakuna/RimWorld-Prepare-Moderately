using System;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasAnyBodyModification : PawnFilterPart {
		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.def.race.body.AllParts.Any((bodyPart) => pawn.health.hediffSet.HasDirectlyAddedPartFor(bodyPart));

		public override string Summary(PawnFilter filter) => "HasAnyBodyModification".Translate();
	}
}
