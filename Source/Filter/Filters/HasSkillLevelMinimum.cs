using PrepareModerately.GUI;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasSkillLevelMinimum : PawnFilterPart {
		[Serializable]
		public class HasSkillLevelMinimumSerializable : PawnFilterPartSerializable {
			public string skill;
			public int level;

			private HasSkillLevelMinimumSerializable() { } // Parameterless constructor necessary for serialization.

			public HasSkillLevelMinimumSerializable(HasSkillLevelMinimum pawnFilterPart) {
				this.skill = pawnFilterPart.skill.LabelCap;
				this.level = pawnFilterPart.level;
			}

			public override PawnFilterPart Deserialize() => new HasSkillLevelMinimum {
				skill = PawnFilter.allSkills.Find(def => def.LabelCap == this.skill),
				level = this.level
			};
		}

		public override PawnFilterPartSerializable Serialize() => new HasSkillLevelMinimumSerializable(this);

		private SkillDef skill;
		private string buffer;
		private int level;

		public HasSkillLevelMinimum() {
			this.label = "Has skill level minimum:";
			this.skill = SkillDefOf.Shooting;
			this.level = 6;
		}

		public override void DoEditInterface(PawnFilterListing list) {
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
			Widgets.TextFieldNumeric(textFieldRect, ref this.level, ref this.buffer);
		}

		public override bool Matches(Pawn pawn) => pawn.skills.GetSkill(this.skill).levelInt >= this.level;
	}
}
