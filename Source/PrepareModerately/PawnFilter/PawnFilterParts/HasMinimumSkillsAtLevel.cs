using PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasMinimumSkillsAtLevel : PawnFilterPart {
		private const float labelWidthPercentage = 0.2f;

		public int skillCount;
		public int skillLevel;

		public HasMinimumSkillsAtLevel() {
			this.skillCount = 2;
			this.skillLevel = 6;
		}

		public override bool Matches(Pawn pawn) {
			int skillsAtLevel = 0;
			foreach (SkillRecord skill in pawn.skills.skills) {
				if (skill.Level >= this.skillLevel) {
					skillsAtLevel++;
				}
			}
			return skillsAtLevel >= this.skillCount;
		}

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2);

			Rect skillCountRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(skillCountRect.LeftPart(HasMinimumSkillsAtLevel.labelWidthPercentage), "Count (" + this.skillCount + ")");
			this.skillCount = (int) Widgets.HorizontalSlider(skillCountRect.RightPart(1 - HasMinimumSkillsAtLevel.labelWidthPercentage), this.skillCount, 1, 12);

			Rect skillLevelRect = new Rect(rect.x, skillCountRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(skillLevelRect.LeftPart(HasMinimumSkillsAtLevel.labelWidthPercentage), "Level (" + this.skillLevel + ")");
			this.skillLevel = (int) Widgets.HorizontalSlider(skillLevelRect.RightPart(1 - HasMinimumSkillsAtLevel.labelWidthPercentage), this.skillLevel, 1, 20);
		}
	}
}
