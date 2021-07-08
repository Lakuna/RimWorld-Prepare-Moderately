using UnityEngine;
using Verse;

namespace PrepareModerately {
	class PawnFilterPart_AgeRange : PawnFilterPart {
		protected IntRange range;
		protected string buffer;

		public PawnFilterPart_AgeRange() {
			this.label = "Age range:";
			this.range = new IntRange(20, 55);
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.IntRange(rect, 0, ref this.range, 14);
		}

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears <= this.range.max && pawn.ageTracker.AgeBiologicalYears >= this.range.min;

		public override string ToLoadableString() => this.GetType().Name + " " + this.range.min + " " + this.range.max;

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			this.range = new IntRange(int.Parse(parts[1]), int.Parse(parts[2]));
		}
	}
}
