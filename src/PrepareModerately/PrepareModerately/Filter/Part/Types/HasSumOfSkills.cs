using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasSumOfSkills : PawnFilterPart {
#pragma warning restore CA1812
		private const int WidgetRowMaxWidth = 24;

		private IntRange _range;

#pragma warning disable CA1002 // RimWorld requires a `List` to serialize with `Scribe_Collections.Look`.
		private List<SkillDef> _skills;
#pragma warning restore CA1002

		private IEnumerable<SkillDef> AddableSkills =>
			DefDatabase<SkillDef>.AllDefs.Where((def) => !this._skills.Contains(def));

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, MiscUtil.NoUnder(nameof(this._range)));
			Scribe_Collections.Look(ref this._skills, MiscUtil.NoUnder(nameof(this._skills)), LookMode.Def);
		}

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int sum = 0;
			foreach (SkillRecord skill in pawn.skills.skills.Where((s) => this._skills.Contains(s.def))) {
				sum += skill.Level;
			}

			return sum >= this._range.min && sum <= this._range.max;
		}

		public override string Summary(PawnFilter filter) =>
			this._range.min == this._range.max ? "PM.HasSumOfSkills".Translate(this._range.min, string.Join(", ", this._skills.Select((def) => def.label).OrderBy((label) => label).ToArray()))
			: "PM.HasBetweenSumOfSkills".Translate(this._range.min, this._range.max, string.Join(", ", this._skills.Select((def) => def.label).OrderBy((label) => label).ToArray()));

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * (1 + this._skills.Count), out totalAddedListHeight, out Rect headerRemainderRect);

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this._range, SkillRecord.MinLevel * this._skills.Count, SkillRecord.MaxLevel * this._skills.Count);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect addSkillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
			Rect addSkillRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(addSkillRect, "PM.AddSkill".Translate().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(this.AddableSkills.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this._skills.Add(def));
			}

			// List added skills.
			float y = addSkillRect.yMax;
			foreach (SkillDef skill in this._skills.OrderBy((s) => s.label)) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Rect skillRect = new Rect(rect.x, y, rect.width, Text.LineHeight);
#else
				Rect skillRect = new(rect.x, y, rect.width, Text.LineHeight);
#endif
				y = skillRect.yMax; // Increment `y` for next skill.

				// "Remove" button.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Rect widgetRect = new Rect(skillRect.x, skillRect.y, WidgetRowMaxWidth, skillRect.height);
				WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#else
				Rect widgetRect = new(skillRect.x, skillRect.y, WidgetRowMaxWidth, skillRect.height);
				WidgetRow widgetRow = new(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#endif
				if (widgetRow.ButtonIcon(Textures.s_delete, null, GenUI.SubtleMouseoverColor)) {
					_ = this._skills.Remove(skill);
					SoundDefOf.Click.PlayOneShotOnCamera();
				}

				// Label.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Rect labelRect = new Rect(widgetRect.xMax, skillRect.y, Text.CalcSize(skill.LabelCap).x, skillRect.height);
#else
				Rect labelRect = new(widgetRect.xMax, skillRect.y, Text.CalcSize(skill.LabelCap).x, skillRect.height);
#endif
				Widgets.Label(labelRect, skill.LabelCap);
			}
		}

		internal override void Randomize() {
			int skillCount = Rand.Range(2, 5);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._skills = new List<SkillDef>();
#else
			this._skills = [];
#endif
			for (int i = 0; i < skillCount; i++) {
				this._skills.Add(this.AddableSkills.RandomElement());
			}

			int min = SkillRecord.MinLevel * this._skills.Count;
			int max = SkillRecord.MaxLevel * this._skills.Count;
			int mid = (max - min) / 2;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(min, mid), Rand.Range(mid, max));
#else
			this._range = new(Rand.Range(min, mid), Rand.Range(mid, max));
#endif
		}
	}
}
