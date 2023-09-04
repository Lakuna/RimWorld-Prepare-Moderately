using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasSkill : PawnFilterPart {
		private SkillDef skill;

		private int level;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.GetSkill(this.skill).levelInt >= this.level;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading,
					(SkillDef def) => def.LabelCap,
					(SkillDef def) => () => this.skill = def);
			}

			float labelWidthPercentage = 0.2f;
			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "LevelNumber".Translate(this.level).CapitalizeFirst());
#if V1_0 || V1_1 || V1_2 || V1_3
			this.level = (int)Widgets.HorizontalSlider(levelRect.RightPart(1 - labelWidthPercentage), this.level, 1, 20);
#else
			float level = this.level;
			Widgets.HorizontalSlider(levelRect.RightPart(1 - labelWidthPercentage), ref level, new FloatRange(1, 20));
			this.level = (int)level;
#endif
		}

		public override string Summary(PawnFilter filter) => "HasLevelInSkill".Translate(this.level, this.skill.label);

		public override void Randomize() {
			this.skill = DefDatabase<SkillDef>.AllDefsListForReading.RandomElement();
			this.level = Rand.Range(5, 10);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.level, nameof(this.level));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
