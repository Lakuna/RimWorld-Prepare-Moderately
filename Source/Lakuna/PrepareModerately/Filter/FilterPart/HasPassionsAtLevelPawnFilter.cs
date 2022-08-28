using System;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasPassionsAtLevelFilterPart : FilterPart {
		public int count;

		public Passion passion;

		public override bool Matches(Pawn pawn) {
			return pawn.skills.skills.FindAll((SkillRecord skill) => skill.passion == this.passion).Count >= this.count;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight * 2);

			float labelWidthPercentage = 0.2f;
			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(labelWidthPercentage).Rounded(), "Count".Translate(this.count).CapitalizeFirst());
			this.count = (int)Widgets.HorizontalSlider(countRect.RightPart(1 - labelWidthPercentage), this.count, 1, 12);

			Rect passionRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(
					(Passion[])Enum.GetValues(typeof(Passion)),
					(Passion passion) => passion.ToString().CapitalizeFirst(),
					(Passion passion) => () => this.passion = passion);
			}
		}

		public override string Summary(Filter filter) {
			return "HasPassions".Translate(this.count, this.passion.ToString());
		}

		public override void Randomize() {
			this.passion = FilterPart.GetRandomOfEnum(new Passion());
			this.count = Rand.RangeInclusive(3, 6);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Values.Look(ref this.count, nameof(this.count));
		}
	}
}
