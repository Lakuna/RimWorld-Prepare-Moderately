using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoMedicalConditions : PawnFilterPart {
		public PawnFilterPart_NoMedicalConditions() => this.label = "No medical conditions.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}
		public override bool Matches(Pawn pawn) => pawn.health.hediffSet.hediffs.Count < 1;
	}
}
