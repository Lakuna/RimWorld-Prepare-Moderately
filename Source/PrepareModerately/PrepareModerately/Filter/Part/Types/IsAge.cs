using Lakuna.PrepareModerately.UI;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsAge : PawnFilterPart {
		private const int TicksPerDay = 60000;

		private const int TicksPerHour = 2500;

		private const int MinAge =
#if V1_0 || V1_1 || V1_2 || V1_3
			14
#else
			0
#endif
			;

		private const int MaxAge = 120;

		private IntRange range;

		private IntRange daysRange;

		private IntRange hoursRange;

		public override bool Matches(Pawn pawn) {
			if (pawn == null) {
				throw new ArgumentNullException(nameof(pawn));
			}

			if (this.range.max > 0) {
				long yearsOld = pawn.ageTracker.AgeBiologicalYears;
				return yearsOld <= this.range.max && yearsOld >= this.range.min;
			}

			if (this.daysRange.max > 0) {
				long daysOld = pawn.ageTracker.AgeBiologicalTicks / TicksPerDay;
				return daysOld <= this.daysRange.max && daysOld >= this.daysRange.min;
			}

			long hoursOld = pawn.ageTracker.AgeBiologicalTicks / TicksPerHour;
			return hoursOld <= this.hoursRange.max && hoursOld >= this.hoursRange.min;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			int displayedExtraRanges = this.range.max > 0 ? 0 : (this.daysRange.max > 0 ? 1 : 2);
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * displayedExtraRanges, out totalAddedListHeight, out Rect headerRemainderRect);

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this.range, MinAge, MaxAge);

			float labelWidthPercentage = 0.2f;
			if (this.range.max <= 0) {
				Rect daysRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
				Widgets.Label(daysRect.LeftPart(labelWidthPercentage).Rounded(), "Days".Translate().CapitalizeFirst());
				Widgets.IntRange(daysRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.daysRange, 0, 60);

				if (this.daysRange.max <= 0) {
					Rect hoursRect = new Rect(rect.x, daysRect.yMax, rect.width, Text.LineHeight);
					Widgets.Label(hoursRect.LeftPart(labelWidthPercentage).Rounded(), "Hours".Translate().CapitalizeFirst());
					Widgets.IntRange(hoursRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.hoursRange, 0, 24);
				}
			}
		}

		public override string Summary(PawnFilter filter) => this.range.max > 0
			? this.range.min == this.range.max
				? "IsYearsOld".Translate(this.range.min)
				: "IsBetweenYearsOld".Translate(this.range.min, this.range.max)
			: this.daysRange.max > 0
				? this.daysRange.min == this.daysRange.max
					? "IsDaysOld".Translate(this.daysRange.min)
					: "IsBetweenDaysOld".Translate(this.daysRange.min, this.daysRange.max)
				: this.hoursRange.min == this.hoursRange.max
					? "IsHoursOld".Translate(this.hoursRange.min)
					: "IsBetweenHoursOld".Translate(this.hoursRange.min, this.hoursRange.max);

		public override void Randomize() {
			this.range = new IntRange(Rand.Range(14, 30), Rand.Range(30, 100));
			this.daysRange = new IntRange(Rand.Range(0, 30), Rand.Range(30, 45));
			this.hoursRange = new IntRange(Rand.Range(0, 10), Rand.Range(10, 20));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Values.Look(ref this.daysRange, nameof(this.daysRange));
			Scribe_Values.Look(ref this.hoursRange, nameof(this.hoursRange));
		}
	}
}
