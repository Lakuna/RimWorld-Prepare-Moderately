using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_MinimumInterests : PawnFilterPart {
		private int skillCount;
		private string skillCountBuffer;
		private Passion passionLevel;

		public PawnFilterPart_MinimumInterests() {
			this.label = "Minimum interests:\nat level:";
			this.skillCount = 2;
			this.passionLevel = Passion.Minor;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * 2);

			// Skill count field.
			Rect skillCountRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric(skillCountRect, ref this.skillCount, ref this.skillCountBuffer);

			// Add passion chooser button.
			Rect passionButtonRect = new Rect(rect.x, rect.y + skillCountRect.height, rect.width, rect.height / 2);
			if (Widgets.ButtonText(passionButtonRect, this.passionLevel.ToString().CapitalizeFirst())) {
				// Fill dropdown.
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (Passion passionLevel in Enum.GetValues(typeof(Passion))) { options.Add(new FloatMenuOption(passionLevel.ToString().CapitalizeFirst(), () => this.passionLevel = passionLevel)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}
		}

		public override bool Matches(Pawn pawn) {
			int interests = 0;
			foreach (SkillRecord skill in pawn.skills.skills) {
				if (skill.passion >= this.passionLevel) { interests++; }
			}
			return interests > this.skillCount;
		}
	}
}
