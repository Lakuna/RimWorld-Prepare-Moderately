using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsCapableOfEverything : PawnFilterPart {
		private static IEnumerable<WorkTags> _legalWorkTags {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Array values = Enum.GetValues(typeof(WorkTags));
#else
				WorkTags[] values = Enum.GetValues<WorkTags>();
#endif
				return values.OfType<WorkTags>();
			}
		}

		private static IEnumerable<WorkTypeDef> _legalWorkTypes {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				List<WorkTypeDef> empty = new List<WorkTypeDef>();
#else
				List<WorkTypeDef> empty = [];
#endif
				return DefDatabase<WorkTypeDef>.AllDefs ?? empty;
			}
		}

		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.IsCapableOf;

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3
#elif V1_4 || V1_5 || V1_6
			List<WorkTags> disabledWorkTags = new List<WorkTags>();
#else
			List<WorkTags> disabledWorkTags = [];
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			foreach (WorkTags workTags in _legalWorkTags) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes?.DisabledWorkTags.HasFlag(workTags) ?? false) {
					disabledWorkTags.Add(workTags);
				}
			}
#endif

			return _legalWorkTypes.All((def) =>
#if V1_0
				!(pawn.story?.WorkTypeIsDisabled(def) ?? false)
#else
				!pawn.WorkTypeIsDisabled(def)
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				|| disabledWorkTags.Any((workTags) => def.workTags.HasFlag(workTags))
#endif
			);
		}

		public override string Summary(PawnFilter filter) =>
			"PM.IsCapableOfEverything".TranslateFull();
	}
}
