using System;
using System.Collections.Generic;
using System.Linq;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsCapableOfEverything : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<WorkTags> LegalWorkTags =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(WorkTags)).OfType<WorkTags>();
#else
			Enum.GetValues<WorkTags>().OfType<WorkTags>();
#endif

		private static IEnumerable<WorkTypeDef> LegalWorkTypes =>
			DefDatabase<WorkTypeDef>.AllDefs;

		internal override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

#if V1_0
			return LegalWorkTypes.All((def) => !pawn.story.WorkTypeIsDisabled(def));
#elif V1_1 || V1_2 || V1_3
			return LegalWorkTypes.All((def) => !pawn.WorkTypeIsDisabled(def));
#elif V1_4 || V1_5 || V1_6
			List<WorkTags> disabledWorkTags = new List<WorkTags>();
			foreach (WorkTags workTags in LegalWorkTags) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes.DisabledWorkTags.HasFlag(workTags)) {
					disabledWorkTags.Add(workTags);
				}
			}

			return LegalWorkTypes.All((def) => !pawn.WorkTypeIsDisabled(def) || disabledWorkTags.Any((workTags) => def.workTags.HasFlag(workTags)));
#else
			List<WorkTags> disabledWorkTags = [];
			foreach (WorkTags workTags in LegalWorkTags) {
				if (workTags == WorkTags.None) {
					continue;
				}

				if (pawn.genes.DisabledWorkTags.HasFlag(workTags)) {
					disabledWorkTags.Add(workTags);
				}
			}

			return LegalWorkTypes.All((def) => !pawn.WorkTypeIsDisabled(def) || disabledWorkTags.Any((workTags) => def.workTags.HasFlag(workTags)));
#endif
		}

		internal override string Summary(PawnFilter filter) =>
			"PM.IsCapableOfEverything".Translate();

		internal override bool CanCoexistWith(PawnFilterPart other) =>
			other is null ? throw new ArgumentNullException(nameof(other))
			: other.def != PawnFilterPartDefOf.IsCapableOf;
	}
}
