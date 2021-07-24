using PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class IsCapableOf : PawnFilterPart {
		public WorkTags workTag;

		public IsCapableOf() => this.workTag = WorkTags.Firefighting;

		public override bool Matches(Pawn pawn) => !pawn.WorkTagIsDisabled(this.workTag);

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.workTag.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((WorkTags[]) Enum.GetValues(typeof(WorkTags)), (workTag) => workTag.ToString().CapitalizeFirst(), (workTag) => () => this.workTag = workTag);
			}
		}
	}
}
