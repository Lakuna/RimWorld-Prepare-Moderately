using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoPermanentMedicalConditions : PawnFilterPart {
		public PawnFilterPart_NoPermanentMedicalConditions() => this.label = "No permanent medical conditions.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}
		public override bool Matches(Pawn pawn) => !pawn.health.hediffSet.hediffs.Any(hediff => hediff.IsPermanent() || hediff.def.chronic);
	}
}
