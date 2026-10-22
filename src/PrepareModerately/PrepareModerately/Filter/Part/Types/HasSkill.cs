using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasSkill : PawnFilterPart {
		private static IEnumerable<SkillDef> _legalSkills =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<SkillDef>.AllDefs ?? new List<SkillDef>();
#else
			DefDatabase<SkillDef>.AllDefs ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._skill is null) {
					yield return "Has skill filter part skill is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._skill is null;

		[Obsolete("`level` has been superseded by `range`.")]
		private int _level;

		private IntRange _range;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private SkillDef _skill;
#else
		private SkillDef? _skill;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight).Rounded();

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight).Rounded();
#if V1_0
			if (Widgets.ButtonText(skillRect, this._skill?.LabelCap ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(skillRect, this._skill?.LabelCap.StringSafe() ?? "null")) {
#else
			if (Widgets.ButtonText(skillRect, this._skill?.LabelCap.StringSafe ?? "null")) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalSkills.OrderBy((def) => def.label),
#if V1_0
					(def) => def.LabelCap ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => def.LabelCap.StringSafe(),
#else
					(def) => def.LabelCap.StringSafe,
#endif
					(def) => () => this._skill = def);
			}

			Widgets.IntRange(new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight).Rounded(), Rand.Int, ref this._range, 0, 20, "PM.LevelRange");
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, "range");
			Scribe_Defs.Look(ref this._skill, "skill");

#pragma warning disable CS0618 // Support loading old filters that use `level` instead of `range`.
			if (Scribe.mode == LoadSaveMode.LoadingVars) {
				Scribe_Values.Look(ref this._level, "level");
			}

			if (Scribe.mode == LoadSaveMode.PostLoadInit && this._range == default) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				this._range = new IntRange(this._level, this._level);
#else
				this._range = new(this._level, this._level);
#endif
			}
#pragma warning restore CS0618
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._range.GetHashCode() ^ (this._skill?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) {
			int level = pawn.skills?.GetSkill(this._skill)?.levelInt ?? 0;
			return level <= this._range.max && level >= this._range.min;
		}

		public override void Randomize() {
			this._skill = _legalSkills.RandomElement();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
#else
			this._range = new(Rand.Range(0, 5), Rand.Range(5, 10));
#endif
		}

		public override string Summary(PawnFilter filter) =>
			(this._range.min == this._range.max ? "PM.IsLevelAtSkill".Translate(this._range.min, this._skill?.label ?? "null")
				: "PM.IsBetweenLevelsAtSkill".Translate(this._range.min, this._range.max, this._skill?.label ?? "null"))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
