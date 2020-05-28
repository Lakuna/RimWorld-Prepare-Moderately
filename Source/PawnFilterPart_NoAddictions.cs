using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoAddictions : PawnFilterPart {
		public PawnFilterPart_NoAddictions() => this.label = "No addictions.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}
		public override bool Matches(Pawn pawn) => !pawn.health.hediffSet.hediffs.Any(hediff => hediff.def.IsAddiction);

		public override string ToLoadableString() => this.GetType().Name;

		public override void FromLoadableString(string s) { }
	}
}
