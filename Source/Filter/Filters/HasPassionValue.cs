using PrepareModerately.GUI;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasPassionValue : PawnFilterPart {
		[Serializable]
		public class HasPassionValueSerializable : PawnFilterPartSerializable {
			public string skill;
			public int passionLevel;

			private HasPassionValueSerializable() { } // Parameterless constructor necessary for serialization.

			public HasPassionValueSerializable(HasPassionValue pawnFilterPart) {
				this.skill = pawnFilterPart.skill.LabelCap;
				this.passionLevel = (int) pawnFilterPart.passionLevel;
			}

			public override PawnFilterPart Deserialize() => new HasPassionValue {
				skill = PawnFilter.allSkills.Find(def => def.LabelCap == this.skill),
				passionLevel = (Passion) this.passionLevel
			};
		}

		public override PawnFilterPartSerializable Serialize() => new HasPassionValueSerializable(this);

		private SkillDef skill;
		private Passion passionLevel;

		public HasPassionValue() {
			this.label = "Has passion value:";
			this.skill = SkillDefOf.Shooting;
			this.passionLevel = Passion.Minor;
		}

		public override void DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * 2);

			// Add skill chooser button.
			Rect skillButtonRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			if (Widgets.ButtonText(skillButtonRect, this.skill.LabelCap)) {
				// Fill dropdown.
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (SkillDef def in PawnFilter.allSkills) { options.Add(new FloatMenuOption(def.LabelCap, () => this.skill = def)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}

			// Add passion chooser button.
			Rect passionButtonRect = new Rect(rect.x, rect.y + skillButtonRect.height, rect.width, rect.height / 2);
			if (Widgets.ButtonText(passionButtonRect, this.passionLevel.ToString().CapitalizeFirst())) {
				// Fill dropdown.
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (Passion passionLevel in Enum.GetValues(typeof(Passion))) { options.Add(new FloatMenuOption(passionLevel.ToString().CapitalizeFirst(), () => this.passionLevel = passionLevel)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}
		}

		public override bool Matches(Pawn pawn) => pawn.skills.GetSkill(this.skill).passion == this.passionLevel;
	}
}
