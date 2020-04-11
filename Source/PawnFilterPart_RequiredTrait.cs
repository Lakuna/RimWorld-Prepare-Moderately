using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_RequiredTrait : PawnFilterPart {
		protected TraitDef trait;

		public PawnFilterPart_RequiredTrait() {
			this.label = "Required trait:";
			this.trait = TraitDefOf.Cannibal;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, ScenPart.RowHeight);

			// Don't do anything when the button isn't clicked.
			if (!Widgets.ButtonText(rect, this.trait.defName.CapitalizeFirst())) { return; }

			// Fill dropdown.
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (TraitDef def in PawnFilter.allTraits) { options.Add(new FloatMenuOption(def.defName.CapitalizeFirst(), () => this.trait = def)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) => pawn.story.traits.allTraits.Find(trait => trait.def == this.trait) != null;
	}
}
