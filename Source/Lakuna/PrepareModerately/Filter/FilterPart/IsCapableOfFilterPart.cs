using System;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsCapableOfFilterPart : FilterPart {
		public WorkTags workTag;

		public override bool Matches(Pawn pawn) {
			return !pawn.WorkTagIsDisabled(this.workTag);
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.workTag.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((WorkTags[])Enum.GetValues(typeof(WorkTags)),
					(WorkTags workTag) => workTag.ToString().CapitalizeFirst(),
					(WorkTags workTag) => () => this.workTag = workTag);
			}
		}

		public override string Summary(Filter filter) {
			return "Is capable of " + this.workTag.ToString() + ".";
		}

		public override void Randomize() {
			this.workTag = ((WorkTags[])Enum.GetValues(typeof(WorkTags)))[Enum.GetValues(typeof(WorkTags)).Length - 1];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.workTag, nameof(this.workTag));
		}

		public override bool CanCoexistWith(FilterPart other) {
			return other.def != FilterPartDefOf.IsCapableOfEverything;
		}
	}
}
