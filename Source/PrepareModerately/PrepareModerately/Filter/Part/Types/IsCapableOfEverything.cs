using System;
using System.Linq;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsCapableOfEverything : PawnFilterPart {

		public override bool Matches(Pawn pawn) => DefDatabase<WorkTypeDef>.AllDefsListForReading.All((WorkTypeDef def) =>
#if V1_0
			!pawn.story.WorkTypeIsDisabled(def));
#else
			!pawn.WorkTypeIsDisabled(def));
#endif

		public override string Summary(PawnFilter filter) => "IsCapableOfEverything".Translate();

		public override bool CanCoexistWith(PawnFilterPart other) => other == null
			? throw new ArgumentNullException(nameof(other))
			: other.Def != PawnFilterPartDefOf.IsCapableOf;
	}
}
