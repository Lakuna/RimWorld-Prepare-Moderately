using PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasMinimumPassionsAtLevel : PawnFilterPart {
		private const float labelWidthPercentage = 0.2f;

		public int count;
		public Passion passion;

		public HasMinimumPassionsAtLevel() {
			this.count = 2;
			this.passion = Passion.Minor;
		}

		public override bool Matches(Pawn pawn) {
			int interestsAtLevel = 0;
			foreach (SkillRecord skill in pawn.skills.skills) {
				if (skill.passion == this.passion) {
					interestsAtLevel++;
				}
			}
			return interestsAtLevel >= this.count;
		}

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2);

			Rect skillCountRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(skillCountRect.LeftPart(HasMinimumPassionsAtLevel.labelWidthPercentage), "Count (" + this.count + ")");
			this.count = (int) Widgets.HorizontalSlider(skillCountRect.RightPart(1 - HasMinimumPassionsAtLevel.labelWidthPercentage), this.count, 1, 20);

			Rect passionRect = new Rect(rect.x, skillCountRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Passion[]) Enum.GetValues(typeof(Passion)), (passion) => passion.ToString().CapitalizeFirst(), (passion) => () => this.passion = passion);
			}
		}
	}
}
