using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NameStartsWith : PawnFilterPart {
		private string startsWith;

		public PawnFilterPart_NameStartsWith() {
			this.label = "Name starts with:";
			this.startsWith = "";
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, 31);
			this.startsWith = Widgets.TextArea(rect, this.startsWith);
		}

		public override bool Matches(Pawn pawn) => pawn.Name.ToStringFull.StartsWith(this.startsWith);
	}
}
