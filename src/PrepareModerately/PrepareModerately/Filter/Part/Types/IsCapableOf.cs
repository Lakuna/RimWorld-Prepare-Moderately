using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsCapableOf : PawnFilterPart {
		private static IEnumerable<WorkTags> LegalWorkTags =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(WorkTags)).OfType<WorkTags>();
#else
			Enum.GetValues<WorkTags>().OfType<WorkTags>();
#endif

		private WorkTags workTag;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0
			: !pawn.story.WorkTagIsDisabled(this.workTag);
#elif V1_1 || V1_2 || V1_3
			: !pawn.WorkTagIsDisabled(this.workTag);
#else
			: !pawn.WorkTagIsDisabled(this.workTag) || pawn.genes.DisabledWorkTags.HasFlag(this.workTag);
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		public override bool NotMatches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.WorkTagIsDisabled(this.workTag);
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.workTag.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalWorkTags.OrderBy((workTag) => workTag.ToString()),
					(workTag) => workTag.ToString().CapitalizeFirst(),
					(workTag) => () => this.workTag = workTag);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.IsCapableOfWorkTag".Translate(this.workTag.ToString());

		public override void Randomize() => this.workTag = LegalWorkTags.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.workTag, nameof(this.workTag));
		}

		public override bool CanCoexistWith(PawnFilterPart other) => other is null
			? throw new ArgumentNullException(nameof(other))
			: other.PDef != PawnFilterPartDefOf.IsCapableOfEverything;
	}
}
