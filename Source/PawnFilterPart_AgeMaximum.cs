using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_AgeMaximum : PawnFilterPart {
		protected int age;
		protected string buffer;

		public PawnFilterPart_AgeMaximum() {
			this.label = "Maximum age:";
			this.age = 55;
			this.buffer = "";
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.TextFieldNumeric(rect, ref this.age, ref this.buffer);
		}

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears <= this.age;

		public override string ToLoadableString() => this.GetType().Name + " " + this.age;

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			this.age = int.Parse(parts[1]);
		}
	}
}
