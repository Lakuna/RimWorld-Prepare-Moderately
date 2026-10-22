using System;
using System.Collections.Generic;
using System.Linq;

#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsCapableOfEverything : PawnFilterPart {
		private static IEnumerable<WorkTags> _legalWorkTags =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(WorkTags)).OfType<WorkTags>();
#else
			Enum.GetValues<WorkTags>().OfType<WorkTags>();
#endif

		private static IEnumerable<WorkTypeDef> _legalWorkTypes =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<WorkTypeDef>.AllDefs ?? new List<WorkTypeDef>();
#else
			DefDatabase<WorkTypeDef>.AllDefs ?? [];
#endif

		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.IsCapableOf;

		public override bool Matches(Pawn pawn) {
#if V1_0
			return _legalWorkTypes.All((def) => !(pawn.story?.WorkTypeIsDisabled(def) ?? false));
#elif V1_1 || V1_2 || V1_3
			return _legalWorkTypes.All((def) => !pawn.WorkTypeIsDisabled(def));
#elif V1_4 || V1_5 || V1_6
			List<WorkTags> disabledWorkTags = new List<WorkTags>();
			foreach (WorkTags workTags in _legalWorkTags) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes?.DisabledWorkTags.HasFlag(workTags) ?? false) {
					disabledWorkTags.Add(workTags);
				}
			}

			return _legalWorkTypes.All((def) => !pawn.WorkTypeIsDisabled(def) || disabledWorkTags.Any((workTags) => def.workTags.HasFlag(workTags)));
#else
			List<WorkTags> disabledWorkTags = [];
			foreach (WorkTags workTags in _legalWorkTags) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes?.DisabledWorkTags.HasFlag(workTags) ?? false) {
					disabledWorkTags.Add(workTags);
				}
			}

			return _legalWorkTypes.All((def) => !pawn.WorkTypeIsDisabled(def) || disabledWorkTags.Any((workTags) => def.workTags.HasFlag(workTags)));
#endif
		}

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.IsCapableOfEverything".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.IsCapableOfEverything".Translate().StringSafe();
#else
			"PM.IsCapableOfEverything".Translate().StringSafe;
#endif
	}
}
