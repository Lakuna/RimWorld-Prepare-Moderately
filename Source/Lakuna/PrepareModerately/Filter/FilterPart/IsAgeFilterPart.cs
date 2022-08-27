using Lakuna.PrepareModerately.UI;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsAgeFilterPart : FilterPart {
		public IntRange range;

		public override bool Matches(Pawn pawn) {
			return pawn.ageTracker.AgeBiologicalYears <= this.range.max && pawn.ageTracker.AgeBiologicalYears >= this.range.min;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			Widgets.IntRange(rect, Rand.Int, ref this.range, 14, 120);
		}

		public override string Summary(Filter filter) {
			return "IsBetweenYearsOld".Translate(this.range.min, this.range.max);
		}

		public override void Randomize() {
			this.range = new IntRange(Rand.RangeInclusive(14, 30), Rand.RangeInclusive(30, 60));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
		}
	}
}
