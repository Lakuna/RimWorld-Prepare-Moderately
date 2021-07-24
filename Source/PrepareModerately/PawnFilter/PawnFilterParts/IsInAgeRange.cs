using PrepareModerately.UI;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class IsInAgeRange : PawnFilterPart {
		private static readonly System.Random random = new System.Random();

		private IntRange range;

		// For serialization.
		public int RangeMin {
			get => this.range.min;
			set => this.range.min = value;
		}

		// For serialization.
		public int RangeMax {
			get => this.range.max;
			set => this.range.max = value;
		}

		public IsInAgeRange() => this.range = new IntRange(20, 55);

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears <= this.range.max && pawn.ageTracker.AgeBiologicalYears >= this.range.min;

		public override void DoEditInterface(Listing_PawnFilter listing) => Widgets.IntRange(listing.GetPawnFilterPartRect(this, Text.LineHeight), random.Next(0x0, 0xFFFFF), ref this.range, 14);
	}
}
