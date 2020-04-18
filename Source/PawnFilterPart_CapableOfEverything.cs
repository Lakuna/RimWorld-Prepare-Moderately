using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_CapableOfEverything : PawnFilterPart {
		public PawnFilterPart_CapableOfEverything() => this.label = "Capable of everything.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}
		public override bool Matches(Pawn pawn) {
			foreach (WorkTypeDef def in PawnFilter.allWorkTypes) {
				if (pawn.WorkTypeIsDisabled(def)) { return false; }
			}
			return true;
		}
	}
}
