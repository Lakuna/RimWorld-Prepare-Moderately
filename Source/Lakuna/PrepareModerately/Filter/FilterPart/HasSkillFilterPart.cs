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
			if (Widgets.ButtonText(skillRect, this.skill.ToString())) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading,
					(SkillDef def) => def.ToString(),
					(SkillDef def) => () => this.skill = def);
			}

			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(0.2f).Rounded(), "Level".Translate(this.level));
			this.level = (int)Widgets.HorizontalSlider(levelRect.RightPart(1 - 0.2f), this.level, 1, 20);
		}

		public override string Summary(Filter filter) {
			return "HasLevelInSkill".Translate(this.level, this.skill.ToString());
		}

		public override void Randomize() {
			this.skill = DefDatabase<SkillDef>.AllDefsListForReading[Rand.RangeInclusive(0, DefDatabase<SkillDef>.AllDefsListForReading.Count - 1)];

			this.level = Rand.RangeInclusive(5, 10);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.level, nameof(this.level));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
