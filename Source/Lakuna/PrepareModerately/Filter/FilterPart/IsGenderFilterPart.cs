using System;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsGenderFilterPart : FilterPart {
		public Gender gender;

		public override bool Matches(Pawn pawn) {
			return pawn.gender == this.gender;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Gender[])Enum.GetValues(typeof(Gender)),
				(Gender gender) => gender.ToString().CapitalizeFirst(),
				(Gender gender) => () => this.gender = gender);
			}
		}

		public override string Summary(Filter filter) {
			return "Is " + this.gender.ToString() + ".";
		}

		public override void Randomize() {
			this.gender = ((Gender[])Enum.GetValues(typeof(Gender)))[Enum.GetValues(typeof(Gender)).Length - 1];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.gender, nameof(this.gender));
		}
	}
}
