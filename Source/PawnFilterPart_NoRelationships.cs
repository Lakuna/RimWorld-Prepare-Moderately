using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoRelationships : PawnFilterPart {
		public PawnFilterPart_NoRelationships() => this.label = "No relationships.";

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}

		public override bool Matches(Pawn pawn) => !pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;

		public override string ToLoadableString() => this.GetType().Name;

		public override void FromLoadableString(string s) { }
	}
}
