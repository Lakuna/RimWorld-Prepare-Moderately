using System;
using System.Collections.Generic;
using System.Linq;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsCapableOfEverything : PawnFilterPart {
		private static IEnumerable<WorkTags> LegalWorkTags => Enum.GetValues(typeof(WorkTags)).OfType<WorkTags>();

		private static IEnumerable<WorkTypeDef> LegalWorkTypes => DefDatabase<WorkTypeDef>.AllDefs;

		public override bool Matches(Pawn pawn) {
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			List<WorkTags> disabledWorkTags = new List<WorkTags>();
			foreach (WorkTags workTags in LegalWorkTags) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes.DisabledWorkTags.HasFlag(workTags)) {
					disabledWorkTags.Add(workTags);
				}
			}
#endif

			return LegalWorkTypes.All((def) =>
#if V1_0
				!pawn.story.WorkTypeIsDisabled(def)
#else
				!pawn.WorkTypeIsDisabled(def)
#if !(V1_1 || V1_2 || V1_3)
				|| disabledWorkTags.Any((workTags) => def.workTags.HasFlag(workTags))
#endif
#endif
				);
		}

		public override string Summary(PawnFilter filter) => "PM.IsCapableOfEverything".Translate();

		public override bool CanCoexistWith(PawnFilterPart other) => other is null
			? throw new ArgumentNullException(nameof(other))
			: other.Def != PawnFilterPartDefOf.IsCapableOf;
	}
}
