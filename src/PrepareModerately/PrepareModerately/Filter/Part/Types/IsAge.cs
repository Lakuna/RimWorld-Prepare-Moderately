using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsAge : PawnFilterPart {
		private const int _maxAge = 120;

		private const int _minAge = 0;

		private const int _ticksPerDay = 60000;

		private const int _ticksPerHour = 2500;

		private IntRange _daysRange;

		private IntRange _hoursRange;

		private IntRange _yearsRange;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			bool showDays = this._yearsRange.max < 1;
			bool showHours = showDays && this._daysRange.max < 1;
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * (1 + (showDays ? 1 : 0) + (showHours ? 1 : 0)), out totalAddedListHeight, out Rect headerRemainderRect).Rounded();

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this._yearsRange, _minAge, _maxAge, "PM.RangeYears");

			if (showDays) {
				Rect daysRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight).Rounded();
				Widgets.IntRange(daysRect, Rand.Int, ref this._daysRange, 0, 60, "PM.RangeDays");

				if (showHours) {
					Widgets.IntRange(new Rect(rect.x, daysRect.yMax, rect.width, Text.LineHeight).Rounded(), Rand.Int, ref this._hoursRange, 0, 24, "PM.RangeHours");
				}
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._yearsRange, "range");
			Scribe_Values.Look(ref this._daysRange, "daysRange");
			Scribe_Values.Look(ref this._hoursRange, "hoursRange");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._daysRange.GetHashCode() ^ this._hoursRange.GetHashCode() ^ this._yearsRange.GetHashCode();

		public override bool Matches(Pawn pawn) {
			if (this._yearsRange.max > 0) {
				int yearsOld = pawn.ageTracker?.AgeBiologicalYears ?? 0;
				return yearsOld <= this._yearsRange.max && yearsOld >= this._yearsRange.min;
			}

			if (this._daysRange.max > 0) {
				long daysOld = (pawn.ageTracker?.AgeBiologicalTicks ?? 0) / _ticksPerDay;
				return daysOld <= this._daysRange.max && daysOld >= this._daysRange.min;
			}

			long hoursOld = (pawn.ageTracker?.AgeBiologicalTicks ?? 0) / _ticksPerHour;
			return hoursOld <= this._hoursRange.max && hoursOld >= this._hoursRange.min;
		}

		public override void Randomize() {
			int min0 = Rand.Range(14, 30);
			int max0 = Rand.Range(30, 100);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._yearsRange = new IntRange(min0, max0);
#else
			this._yearsRange = new(min0, max0);
#endif
			int min1 = Rand.Range(0, 30);
			int max1 = Rand.Range(30, 45);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._daysRange = new IntRange(min1, max1);
#else
			this._daysRange = new(min1, max1);
#endif
			int min2 = Rand.Range(0, 10);
			int max2 = Rand.Range(10, 20);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._hoursRange = new IntRange(min2, max2);
#else
			this._hoursRange = new(min2, max2);
#endif
		}

		public override string Summary(PawnFilter filter) =>
			this._yearsRange.max > 0 ?
				this._yearsRange.min == this._yearsRange.max ? "PM.IsYearsOld".TranslateFull(this._yearsRange.min)
				: "PM.IsBetweenYearsOld".TranslateFull(this._yearsRange.min, this._yearsRange.max)
			: this._daysRange.max > 0 ?
				this._daysRange.min == this._daysRange.max ? "PM.IsDaysOld".TranslateFull(this._daysRange.min)
				: "PM.IsBetweenDaysOld".TranslateFull(this._daysRange.min, this._daysRange.max)
			: this._hoursRange.min == this._hoursRange.max ? "PM.IsHoursOld".TranslateFull(this._hoursRange.min)
			: "PM.IsBetweenHoursOld".TranslateFull(this._hoursRange.min, this._hoursRange.max);
	}
}
