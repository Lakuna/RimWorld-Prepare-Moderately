using PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasRelationship : PawnFilterPart {
		private PawnRelationDef relation;

		// For serialization.
		public string RelationLabelCap {
			get => this.relation.ToString().CapitalizeFirst();
			set => this.relation = DefDatabase<PawnRelationDef>.AllDefsListForReading.Find((def) => def.ToString().CapitalizeFirst() == value);
		}

		public HasRelationship() => this.relation = PawnRelationDefOf.Child;

		public override bool Matches(Pawn pawn) => pawn.relations.DirectRelations.Find((relation) => relation.def == this.relation) != null;

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.RelationLabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<PawnRelationDef>.AllDefsListForReading, (def) => def.ToString().CapitalizeFirst(), (def) => () => this.relation = def);
			}
		}
	}
}
