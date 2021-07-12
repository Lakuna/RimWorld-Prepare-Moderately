using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_HasRelationship : PawnFilterPart {
		[Serializable]
		public class SerializableHasRelationship : SerializablePawnFilterPart {
			public string relation;

			public SerializableHasRelationship(PawnFilterPart_HasRelationship pawnFilterPart) => this.relation = pawnFilterPart.relation.LabelCap;

			public override PawnFilterPart Deserialize() => new PawnFilterPart_HasRelationship {
				relation = PawnFilter.allRelations.Find(def => def.LabelCap == this.relation)
			};
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableHasRelationship(this);

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
	}
}
