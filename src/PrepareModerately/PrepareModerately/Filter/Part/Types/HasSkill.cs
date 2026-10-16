using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasSkill : PawnFilterPart {
#pragma warning restore CA1812
		private const float LabelWidthPercentage = 0.2f;

		private static IEnumerable<SkillDef> LegalSkills =>
			DefDatabase<SkillDef>.AllDefs;

		[Obsolete("`level` has been superseded by `range`.")]
		private int _level;

		private IntRange _range;

		private SkillDef _skill;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, MiscUtil.NoUnder(nameof(this._range)));
			Scribe_Defs.Look(ref this._skill, MiscUtil.NoUnder(nameof(this._skill)));

#pragma warning disable CS0618 // `level` has been superseded by `range`.
			if (Scribe.mode == LoadSaveMode.LoadingVars) {
				Scribe_Values.Look(ref this._level, MiscUtil.NoUnder(nameof(this._level)));
			}

			if (Scribe.mode == LoadSaveMode.PostLoadInit && this._range == default) {
				this._range.min = this._level;
				this._range.max = this._level;
			}
#pragma warning restore CS0618
		}

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int level = pawn.skills.GetSkill(this._skill).levelInt;
			return level <= this._range.max && level >= this._range.min;
		}

		public override string Summary(PawnFilter filter) =>
			this._range.min == this._range.max ? "PM.IsLevelAtSkill".Translate(this._range.min, this._skill.label)
			: "PM.IsBetweenLevelsAtSkill".Translate(this._range.min, this._range.max, this._skill.label);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
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
			if (Widgets.ButtonText(skillRect, this._skill.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalSkills.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this._skill = def);
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect levelRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#else
			Rect levelRect = new(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
#endif
			Widgets.Label(levelRect.LeftPart(LabelWidthPercentage).Rounded(), "PM.Level".Translate().CapitalizeFirst());
			Widgets.IntRange(levelRect.RightPart(1 - LabelWidthPercentage), Rand.Int, ref this._range, 0, 20);
		}

		internal override void Randomize() {
			this._skill = LegalSkills.RandomElement();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
#else
			this._range = new(Rand.Range(0, 5), Rand.Range(5, 10));
#endif
		}
	}
}
