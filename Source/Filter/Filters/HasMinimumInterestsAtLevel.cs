using PrepareModerately.GUI;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasMinimumInterestsAtLevel : PawnFilterPart {
		[Serializable]
		public class HasMinimumInterestsAtLevelSerializable : PawnFilterPartSerializable {
			public int passion;

			private HasMinimumInterestsAtLevelSerializable() { } // Parameterless constructor necessary for serialization.

			public HasMinimumInterestsAtLevelSerializable(HasMinimumInterestsAtLevel pawnFilterPart) => this.passion = (int) pawnFilterPart.passion;

			public override PawnFilterPart Deserialize() => new HasMinimumInterestsAtLevel {
				passion = (Passion) this.passion
			};
		}

		public override PawnFilterPartSerializable Serialize() => new HasMinimumInterestsAtLevelSerializable(this);

		private int count;
		private string buffer;
		private Passion passion;

		public HasMinimumInterestsAtLevel() {
			this.label = "Has minimum interests at level:";
			this.count = 2;
			this.passion = Passion.Minor;
		}

		public override void DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * 2);

			// Skill count field.
			Rect skillCountRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric(skillCountRect, ref this.count, ref this.buffer);

			// Add passion chooser button.
			Rect passionButtonRect = new Rect(rect.x, rect.y + skillCountRect.height, rect.width, rect.height / 2);
			if (Widgets.ButtonText(passionButtonRect, this.passion.ToString().CapitalizeFirst())) {
				// Fill dropdown.
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (Passion passionLevel in Enum.GetValues(typeof(Passion))) { options.Add(new FloatMenuOption(passionLevel.ToString().CapitalizeFirst(), () => this.passion = passionLevel)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}
		}

		public override bool Matches(Pawn pawn) {
			int interests = 0;
			foreach (SkillRecord skill in pawn.skills.skills) {
				if (skill.passion >= this.passion) { interests++; }
			}
			return interests >= this.count;
		}
	}
}
