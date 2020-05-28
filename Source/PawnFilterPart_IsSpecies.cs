using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_IsSpecies : PawnFilterPart {
		private ThingDef humanlike;

		public PawnFilterPart_IsSpecies() {
			this.label = "Is species:";
			this.humanlike = ThingDefOf.Human;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			if (!Widgets.ButtonText(rect, this.humanlike.LabelCap)) { return; }

			// Fill humanlike dropdown when button is pressed.
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (ThingDef def in PawnFilter.AllHumanlikeDefs) { options.Add(new FloatMenuOption(def.LabelCap, () => this.humanlike = def)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) => pawn.def == this.humanlike;

		public override string ToLoadableString() => this.GetType().Name + " " + this.humanlike.LabelCap;

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			this.humanlike = PawnFilter.AllHumanlikeDefs.Find(def => def.LabelCap == parts[1]);
		}
	}
}
