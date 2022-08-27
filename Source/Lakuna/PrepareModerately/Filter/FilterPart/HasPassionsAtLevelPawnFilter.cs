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

			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(0.2f).Rounded(), "Count (" + this.count + ")");
			this.count = (int)Widgets.HorizontalSlider(countRect.RightPart(1 - 0.2f), this.count, 1, 12);

			Rect passionRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(
					(Passion[])Enum.GetValues(typeof(Passion)),
					(Passion passion) => passion.ToString().CapitalizeFirst(),
					(Passion passion) => () => this.passion = passion);
			}
		}

		public override string Summary(Filter filter) {
			return "Has " + this.count + " " + this.passion.ToString() + " passions.";
		}

		public override void Randomize() {
			switch (Rand.RangeInclusive(0, 2)) {
				case 0:
					this.passion = Passion.None;
					break;
				case 1:
					this.passion = Passion.Minor;
					break;
				case 2:
					this.passion = Passion.Major;
					break;
			}

			this.count = Rand.RangeInclusive(3, 6);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Values.Look(ref this.count, nameof(this.count));
		}
	}
}
