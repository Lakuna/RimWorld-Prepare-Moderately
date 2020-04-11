using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_CapableOf : PawnFilterPart {
		private SkillDef skill;

		public PawnFilterPart_CapableOf() {
			this.label = "Capable of:";
			this.skill = SkillDefOf.Shooting;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, ScenPart.RowHeight);

			// Don't do anything when the button isn't clicked.
			if (!Widgets.ButtonText(rect, this.skill.LabelCap)) { return; }

			// Fill dropdown.
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (SkillDef def in PawnFilter.allSkills) { options.Add(new FloatMenuOption(def.LabelCap, () => this.skill = def)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) => !pawn.skills.GetSkill(this.skill).TotallyDisabled;
	}
}
