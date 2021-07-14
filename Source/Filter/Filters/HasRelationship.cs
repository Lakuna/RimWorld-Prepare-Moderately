using PrepareModerately.GUI;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasRelationship : PawnFilterPart {
		[Serializable]
		public class HasRelationshipSerializable : PawnFilterPartSerializable {
			public string relation;

			private HasRelationshipSerializable() { } // Parameterless constructor necessary for serialization.

			public HasRelationshipSerializable(HasRelationship pawnFilterPart) => this.relation = pawnFilterPart.relation.LabelCap;

			public override PawnFilterPart Deserialize() => new HasRelationship {
				relation = PawnFilter.allRelations.Find(def => def.LabelCap == this.relation)
			};
		}

		public override PawnFilterPartSerializable Serialize() => new HasRelationshipSerializable(this);

		private PawnRelationDef relation;

		public HasRelationship() {
			this.label = "Has relationship:";
			this.relation = PawnRelationDefOf.Lover;
		}

		public override float DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);

			if (Widgets.ButtonText(rect, this.relation.LabelCap)) {
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (PawnRelationDef def in PawnFilter.allRelations) { options.Add(new FloatMenuOption(def.LabelCap, () => this.relation = def)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}

			return RowHeight;
		}

		public override bool Matches(Pawn pawn) {
			DirectPawnRelation matchedRelation = pawn.relations.DirectRelations.Find(relation => relation.def == this.relation);
			return matchedRelation != null;
		}
	}
}
