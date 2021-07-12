using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_MinimumInterests : PawnFilterPart {
		private int count;
		private string buffer;
		private Passion passion;

		public PawnFilterPart_MinimumInterests() {
			this.label = "Minimum interests at level:";
			this.count = 2;
			this.passion = Passion.Minor;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
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
			return interests > this.count;
		}

		public override string ToLoadableString() => this.GetType().Name + " " + this.count + " " + (int) this.passion;

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			this.count = int.Parse(parts[1]);
			this.passion = (Passion) int.Parse(parts[2]);
		}
	}
}
