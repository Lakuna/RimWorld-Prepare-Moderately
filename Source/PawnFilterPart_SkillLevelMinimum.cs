using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_SkillLevelMinimum : PawnFilterPart {
		private SkillDef skill;
		private string buffer;
		private int level;

		public PawnFilterPart_SkillLevelMinimum() {
			this.label = "Skill level minimum:";
			this.skill = SkillDefOf.Shooting;
			this.buffer = "";
			this.level = 0;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * 2);

			// Add skill chooser button.
			Rect buttonRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			if (Widgets.ButtonText(buttonRect, this.skill.LabelCap)) {
				// Fill dropdown.
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (SkillDef def in PawnFilter.allSkills) { options.Add(new FloatMenuOption(def.LabelCap, () => this.skill = def)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}

			// Add level input field.
			Rect textFieldRect = new Rect(rect.x, rect.y + buttonRect.height, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric<int>(textFieldRect, ref this.level, ref this.buffer);
		}

		public override bool Matches(Pawn pawn) => pawn.skills.GetSkill(this.skill).levelInt >= this.level;
	}
}
