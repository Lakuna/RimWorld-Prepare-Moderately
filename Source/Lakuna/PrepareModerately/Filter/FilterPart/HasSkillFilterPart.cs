using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasSkillFilterPart : FilterPart {
		public SkillDef skill;

		public int level;

		public override bool Matches(Pawn pawn) {
			return pawn.skills.GetSkill(this.skill).levelInt >= this.level;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight * 2);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading,
					(SkillDef def) => def.LabelCap,
					(SkillDef def) => () => this.skill = def);
			}

			float labelWidthPercentage = 0.2f;
			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "Level".Translate(this.level).CapitalizeFirst());
			this.level = (int)Widgets.HorizontalSlider(levelRect.RightPart(1 - labelWidthPercentage), this.level, 1, 20);
		}

		public override string Summary(Filter filter) {
			return "HasLevelInSkill".Translate(this.level, this.skill.label);
		}

		public override void Randomize() {
			this.skill = FilterPart.GetRandomOfDef(new SkillDef());
			this.level = Rand.Range(5, 10);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.level, nameof(this.level));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
