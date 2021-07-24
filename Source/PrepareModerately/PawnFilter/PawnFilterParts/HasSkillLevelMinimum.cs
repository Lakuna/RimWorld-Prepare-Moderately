using PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasSkillLevelMinimum : PawnFilterPart {
		private const float labelWidthPercentage = 0.2f;

		private SkillDef skill;
		public int level;

		// For serialization.
		public string SkillLabelCap {
			get => this.skill.ToString().CapitalizeFirst();
			set => this.skill = DefDatabase<SkillDef>.AllDefsListForReading.Find((def) => def.ToString().CapitalizeFirst() == value);
		}

		public HasSkillLevelMinimum() {
			this.skill = SkillDefOf.Shooting;
			this.level = 6;
		}

		public override bool Matches(Pawn pawn) => pawn.skills.GetSkill(this.skill).levelInt >= this.level;

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.SkillLabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading, (def) => def.ToString().CapitalizeFirst(), (def) => () => this.skill = def);
			}

			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(HasSkillLevelMinimum.labelWidthPercentage), "Level (" + this.level + ")");
			this.level = (int) Widgets.HorizontalSlider(levelRect.RightPart(1 - HasSkillLevelMinimum.labelWidthPercentage), this.level, 1, 20);
		}
	}
}
