using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasSkill : PawnFilterPart {
		private static IEnumerable<SkillDef> LegalSkills => DefDatabase<SkillDef>.AllDefs;

		private SkillDef skill;

		private IntRange range;

		[Obsolete("`level` has been superseded by `range`.")]
		private int level;

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int level = pawn.skills.GetSkill(this.skill).levelInt;
			return level <= this.range.max && level >= this.range.min;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
			Rect skillRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(skillRect, this.skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalSkills.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.skill = def);
			}

			float labelWidthPercentage = 0.2f;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#else
			Rect levelRect = new(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#endif
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "PM.Level".Translate().CapitalizeFirst());
			Widgets.IntRange(levelRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.range, 0, 20);
		}

		public override string Summary(PawnFilter filter) => this.range.min == this.range.max
			? "PM.IsLevelAtSkill".Translate(this.range.min, this.skill.label)
			: "PM.IsBetweenLevelsAtSkill".Translate(this.range.min, this.range.max, this.skill.label);

		public override void Randomize() {
			this.skill = LegalSkills.RandomElement();
			this.range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Defs.Look(ref this.skill, nameof(this.skill));

#pragma warning disable CS0618 // `level` has been superseded by `range`.
			if (Scribe.mode == LoadSaveMode.LoadingVars) {
				Scribe_Values.Look(ref this.level, nameof(this.level));
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.range == default) {
				this.range.min = this.level;
				this.range.max = this.level;
			}
#pragma warning restore CS0618
		}
	}
}
