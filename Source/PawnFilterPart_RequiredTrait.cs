using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_RequiredTrait : PawnFilterPart {
		protected TraitDef trait;
		protected int degree;

		public PawnFilterPart_RequiredTrait() {
			this.label = "Required trait:";
			this.SelectTrait(TraitDefOf.Cannibal);
		}

		private void SelectTrait(TraitDef def) {
			this.trait = def;
			this.degree = this.trait.degreeDatas[0].degree;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, ScenPart.RowHeight * 2);

			// Trait list.
			Rect traitRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			if (Widgets.ButtonText(traitRect, this.trait.defName.CapitalizeFirst())) {
				// Fill trait dropdown when button is pressed.
				List<FloatMenuOption> traitOptions = new List<FloatMenuOption>();
				foreach (TraitDef def in PawnFilter.allTraits) { traitOptions.Add(new FloatMenuOption(def.defName.CapitalizeFirst(), () => this.SelectTrait(def))); }
				Find.WindowStack.Add(new FloatMenu(traitOptions));
			}

			// Degree list.
			Rect degreeRect = new Rect(rect.x, rect.y + traitRect.height, rect.width, rect.height / 2);
			if (Widgets.ButtonText(degreeRect, this.trait.DataAtDegree(this.degree).LabelCap)) {
				// Fill degree dropdown when button is pressed.
				List<FloatMenuOption> degreeOptions = new List<FloatMenuOption>();
				foreach (TraitDegreeData data in this.trait.degreeDatas) { degreeOptions.Add(new FloatMenuOption(data.LabelCap, () => this.degree = data.degree)); }
				Find.WindowStack.Add(new FloatMenu(degreeOptions));
			}
		}

		public override bool Matches(Pawn pawn) {
			Trait matchedTrait = pawn.story.traits.allTraits.Find(trait => trait.def == this.trait);
			return matchedTrait == null ? false : matchedTrait.Degree == this.degree;
		}
	}
}
