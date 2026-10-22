using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasSumOfSkills : PawnFilterPart {
		private const int _widgetRowMaxWidth = 24;

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._skills is null) {
					yield return "Has sum of skills filter part skills is null.";
				}
			}
		}

		public override bool Valid =>
			base.Valid
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			&& this._skills != null;
#else
			&& this._skills is not null;
#endif

		private IEnumerable<SkillDef> _addableSkills =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<SkillDef>.AllDefs?.Where((def) => !this._skills?.Contains(def) ?? false) ?? new List<SkillDef>();
#else
			DefDatabase<SkillDef>.AllDefs?.Where((def) => !this._skills?.Contains(def) ?? false) ?? [];
#endif

		private IntRange _range;

#pragma warning disable CA1002 // `Scribe_Collections.Look` must be used with a `List`.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private List<SkillDef> _skills = new List<SkillDef>();
#else
		private List<SkillDef>? _skills = [];
#endif
#pragma warning restore CA1002

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * (1 + (this._skills?.Count ?? 0)), out totalAddedListHeight, out Rect headerRemainderRect).Rounded();

			if (this._skills is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw edit interface of has sum of skills filter part with null skills.");
				return;
			}

			if (Textures.Delete is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw null delete texture.");
				return;
			}

			// If the range goes beyond the maximum because a skill was just removed from the list, adjust it to fit on the UI.
			int max = SkillRecord.MaxLevel * this._skills.Count;
			if (this._range.max > max) {
				this._range.max = max;
			}

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this._range, SkillRecord.MinLevel * this._skills.Count, max, "PM.LevelRange");

			Rect addSkillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight).Rounded();
#if V1_0
			if (Widgets.ButtonText(addSkillRect, "PM.AddSkill".Translate().CapitalizeFirst() ?? "null")) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(addSkillRect, "PM.AddSkill".Translate().CapitalizeFirst().StringSafe())) {
#else
			if (Widgets.ButtonText(addSkillRect, "PM.AddSkill".Translate().CapitalizeFirst().StringSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					this._addableSkills.OrderBy((def) => def.label),
#if V1_0
					(def) => def.LabelCap ?? "null",
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => def.LabelCap.StringSafe(),
#else
					(def) => def.LabelCap.StringSafe,
#endif
					(def) => () => this._skills.Add(def));
			}

			// List added skills.
			float y = addSkillRect.yMax;
			foreach (SkillDef skill in this._skills.OrderBy((s) => s.label)) {
				Rect skillRect = new Rect(rect.x, y, rect.width, Text.LineHeight).Rounded();
				y = skillRect.yMax; // Increment `y` for next skill.

				// "Remove" button.
				Rect widgetRect = new Rect(skillRect.x, skillRect.y, _widgetRowMaxWidth, skillRect.height).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				WidgetRow widgetRow = new WidgetRow(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#else
				WidgetRow widgetRow = new(widgetRect.x, widgetRect.y, UIDirection.RightThenDown, widgetRect.width, 0);
#endif
				if (widgetRow.ButtonIcon(Textures.Delete, null, GenUI.SubtleMouseoverColor)) {
					SoundDefOf.Click?.PlayOneShotOnCamera();
					_ = this._skills.Remove(skill);
				}

				// Label.
				Widgets.Label(new Rect(widgetRect.xMax, skillRect.y, Text.CalcSize(skill.LabelCap).x, skillRect.height).Rounded(), skill.LabelCap);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, "range");
			Scribe_Collections.Look(ref this._skills, "skills", LookMode.Def);
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._range.GetHashCode() ^ (this._skills?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) {
			if (this._skills is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to match pawn with has sum of skills filter part with null skills.");
				return false;
			}

			int sum = 0;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			foreach (SkillRecord skill in pawn.skills?.skills?.Where((skill) => skill != null && this._skills.Contains(skill.def)) ?? new List<SkillRecord>()) {
#else
			foreach (SkillRecord skill in pawn.skills?.skills?.Where((skill) => skill is not null && this._skills.Contains(skill.def)) ?? []) {
#endif
				sum += skill.Level;
			}

			return sum >= this._range.min && sum <= this._range.max;
		}

		public override void Randomize() {
			if (this._skills is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to randomize has sum of skills filter part with null skills.");
				return;
			}

			int skillCount = Rand.Range(2, 5);
			this._skills.Clear();
			for (int i = 0; i < skillCount; i++) {
				this._skills.Add(this._addableSkills.RandomElement());
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

		public override string Summary(PawnFilter filter) =>
			this._skills is null ? "null"
			: (this._range.min == this._range.max ? "PM.HasSumOfSkills".Translate(this._range.min, string.Join(", ", this._skills.Select((def) => def.label ?? "null").OrderBy((label) => label).ToArray()))
				: "PM.HasBetweenSumOfSkills".Translate(this._range.min, this._range.max, string.Join(", ", this._skills.Select((def) => def.label ?? "null").OrderBy((label) => label).ToArray())))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
