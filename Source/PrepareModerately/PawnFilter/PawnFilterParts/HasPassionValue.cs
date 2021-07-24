using PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasPassionValue : PawnFilterPart {
		private SkillDef skill;
		public Passion passion;

		// For serialization.
		public string SkillLabelCap {
			get => this.skill.ToString().CapitalizeFirst();
			set => this.skill = DefDatabase<SkillDef>.AllDefsListForReading.Find((def) => def.ToString().CapitalizeFirst() == value);
		}

		public HasPassionValue() {
			this.skill = SkillDefOf.Shooting;
			this.passion = Passion.Minor;
		}

		public override bool Matches(Pawn pawn) => pawn.skills.GetSkill(this.skill).passion == this.passion;

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2);

			Rect skillRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(skillRect, this.SkillLabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<SkillDef>.AllDefsListForReading, (def) => def.ToString().CapitalizeFirst(), (def) => () => this.skill = def);
			}

			Rect passionRect = new Rect(rect.x, skillRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Passion[]) Enum.GetValues(typeof(Passion)), (passion) => passion.ToString().CapitalizeFirst(), (passion) => () => this.passion = passion);
			}
		}
	}
}
