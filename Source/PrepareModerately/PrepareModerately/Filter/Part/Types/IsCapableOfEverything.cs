using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsCapableOfEverything : PawnFilterPart {

		public override bool Matches(Pawn pawn) {
			if (pawn == null) {
				throw new ArgumentNullException(nameof(pawn));
			}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			List<WorkTags> disabledWorkTags = new List<WorkTags>();
			foreach (WorkTags workTags in Enum.GetValues(typeof(WorkTags))) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes.DisabledWorkTags.HasFlag(workTags)) {
					disabledWorkTags.Add(workTags);
				}
			}
#endif

			return DefDatabase<WorkTypeDef>.AllDefsListForReading.All((WorkTypeDef def) =>
#if V1_0
				!pawn.story.WorkTypeIsDisabled(def)
#else
				!pawn.WorkTypeIsDisabled(def)
#if !(V1_1 || V1_2 || V1_3)
				|| disabledWorkTags.Any((WorkTags workTags) => def.workTags.HasFlag(workTags))
#endif
#endif
				);
		}

		public override string Summary(PawnFilter filter) => "IsCapableOfEverything".Translate();

		public override bool CanCoexistWith(PawnFilterPart other) => other == null
			? throw new ArgumentNullException(nameof(other))
			: other.Def != PawnFilterPartDefOf.IsCapableOf;
	}
}
