using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasSkill : PawnFilterPart {
		private SkillDef skill;

		private IntRange range;

		public override bool Matches(Pawn pawn) {
			if (pawn == null) {
				throw new ArgumentNullException(nameof(pawn));
			}

			int level = pawn.skills.GetSkill(this.skill).levelInt;
			return level <= this.range.max && level >= this.range.min;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading,
					(def) => def.LabelCap,
					(def) => () => this.skill = def);
			}

			float labelWidthPercentage = 0.2f;
			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "Level".Translate().CapitalizeFirst());
			Widgets.IntRange(levelRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.range, 0, 20);
		}

		public override string Summary(PawnFilter filter) => this.range.min == this.range.max
			? "IsLevelAtSkill".Translate(this.range.min, this.skill.label)
			: "IsBetweenLevelsAtSkill".Translate(this.range.min, this.range.max, this.skill.label);

		public override void Randomize() {
			this.skill = DefDatabase<SkillDef>.AllDefsListForReading.RandomElement();
			this.range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));
		}
	}
}
