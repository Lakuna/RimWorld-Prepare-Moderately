using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_AgeMaximum : PawnFilterPart {
		private int maximumAge;
		private string buffer;

		public PawnFilterPart_AgeMaximum() {
			this.label = "Maximum age:";
			this.maximumAge = 55;
			this.buffer = "";
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.TextFieldNumeric(rect, ref this.maximumAge, ref this.buffer);
		}

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears <= this.maximumAge;
	}
}
