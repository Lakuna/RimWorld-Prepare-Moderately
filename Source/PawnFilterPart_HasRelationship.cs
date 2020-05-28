using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_HasRelationship : PawnFilterPart {
		private PawnRelationDef relation;

		public PawnFilterPart_HasRelationship() {
			this.label = "Has relationship:";
			this.relation = PawnRelationDefOf.Lover;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			if (!Widgets.ButtonText(rect, this.relation.LabelCap)) { return; }

			// Fill relationship dropdown when button is pressed.
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (PawnRelationDef def in PawnFilter.allRelations) { options.Add(new FloatMenuOption(def.LabelCap, () => this.relation = def)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) {
			DirectPawnRelation matchedRelation = pawn.relations.DirectRelations.Find(relation => relation.def == this.relation);
			return matchedRelation != null;
		}

		public override string ToLoadableString() => this.GetType().Name + " " + this.relation.LabelCap;

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			this.relation = PawnFilter.allRelations.Find(def => def.LabelCap == parts[1]);
		}
	}
}
