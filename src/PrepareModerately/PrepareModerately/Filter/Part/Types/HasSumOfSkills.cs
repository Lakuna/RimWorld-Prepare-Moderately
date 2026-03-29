using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasSumOfSkills : PawnFilterPart {
		private const int WidgetRowMaxWidth = 24;

		private List<SkillDef> skills;

		private IntRange range;

		public override bool Matches(Pawn pawn) {
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}

			int sum = 0;
			foreach (SkillRecord skill in pawn.skills.skills.Where((s) => this.skills.Contains(s.def))) {
				sum += skill.Level;
			}

			return sum >= this.range.min && sum <= this.range.max;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * (1 + this.skills.Count), out totalAddedListHeight, out Rect headerRemainderRect);

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this.range, SkillRecord.MinLevel * this.skills.Count, SkillRecord.MaxLevel * this.skills.Count);

			Rect addSkillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(addSkillRect, "PM.AddSkill".Translate().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading.Where((def) => !this.skills.Contains(def)).OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.skills.Add(def));
			}

			// List added skills.
			float y = addSkillRect.yMax;
			foreach (SkillDef skill in this.skills.OrderBy((s) => s.label)) {
				Rect skillRect = new Rect(rect.x, y, rect.width, Text.LineHeight);
				y = skillRect.yMax; // Increment `y` for next skill.

				// "Remove" button.
				Rect widgetRect = new Rect(skillRect.x, skillRect.y, WidgetRowMaxWidth, skillRect.height);
				WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
				if (widgetRow.ButtonIcon(Textures.DeleteX, null, GenUI.SubtleMouseoverColor)) {
					_ = this.skills.Remove(skill);
					SoundDefOf.Click.PlayOneShotOnCamera();
				}

				// Label.
				Rect labelRect = new Rect(widgetRect.xMax, skillRect.y, Text.CalcSize(skill.LabelCap).x, skillRect.height);
				Widgets.Label(labelRect, skill.LabelCap);
			}
		}

		public override string Summary(PawnFilter filter) => this.range.min == this.range.max
			? "PM.HasSumOfSkills".Translate(this.range.min, string.Join(", ", this.skills.Select((def) => def.label).OrderBy((label) => label)))
			: "PM.HasBetweenSumOfSkills".Translate(this.range.min, this.range.max, string.Join(", ", this.skills.Select((def) => def.label).OrderBy((label) => label)));

		public override void Randomize() {
			int skillCount = Rand.Range(2, 5);

			this.skills = new List<SkillDef>();
			for (int i = 0; i < skillCount; i++) {
				this.skills.Add(DefDatabase<SkillDef>.AllDefsListForReading.Where((def) => !this.skills.Contains(def)).RandomElement());
			}

			int min = SkillRecord.MinLevel * this.skills.Count;
			int max = SkillRecord.MaxLevel * this.skills.Count;
			int mid = (max - min) / 2;
			this.range = new IntRange(Rand.Range(min, mid), Rand.Range(mid, max));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Collections.Look(ref this.skills, nameof(this.skills), LookMode.Deep);
		}
	}
}
