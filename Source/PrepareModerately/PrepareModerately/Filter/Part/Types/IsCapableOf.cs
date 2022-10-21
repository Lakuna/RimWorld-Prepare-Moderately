﻿using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsCapableOf : PawnFilterPart {
		private WorkTags workTag;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_0
			: !pawn.story.WorkTagIsDisabled(this.workTag);
#else
			: !pawn.WorkTagIsDisabled(this.workTag);
#endif


		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.workTag.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((WorkTags[])Enum.GetValues(typeof(WorkTags)),
					(WorkTags workTag) => workTag.ToString().CapitalizeFirst(),
					(WorkTags workTag) => () => this.workTag = workTag);
			}
		}

		public override string Summary(PawnFilter filter) => "IsCapableOfWorkTag".Translate(this.workTag.ToString());

		public override void Randomize() => this.workTag = GetRandomOfEnum(new WorkTags());

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.workTag, nameof(this.workTag));
		}

		public override bool CanCoexistWith(PawnFilterPart other) => other == null
			? throw new ArgumentNullException(nameof(other))
			: other.Def != PawnFilterPartDefOf.IsCapableOfEverything;
	}
}