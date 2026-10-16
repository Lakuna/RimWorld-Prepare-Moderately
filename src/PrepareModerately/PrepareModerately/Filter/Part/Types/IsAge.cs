using System;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsAge : PawnFilterPart {
#pragma warning restore CA1812
		private const int MaxAge = 120;

#if V1_0 || V1_1 || V1_2 || V1_3
		private const int MinAge = 14;
#else
		private const int MinAge = 0;
#endif

		private const int TicksPerDay = 60000;

		private const int TicksPerHour = 2500;

		private IntRange _daysRange;

		private IntRange _hoursRange;

		private IntRange _range;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, MiscUtil.NoUnder(nameof(this._range)));
			Scribe_Values.Look(ref this._daysRange, MiscUtil.NoUnder(nameof(this._daysRange)));
			Scribe_Values.Look(ref this._hoursRange, MiscUtil.NoUnder(nameof(this._hoursRange)));
		}

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			if (this._range.max > 0) {
				long yearsOld = pawn.ageTracker.AgeBiologicalYears;
				return yearsOld <= this._range.max && yearsOld >= this._range.min;
			}

			if (this._daysRange.max > 0) {
				long daysOld = pawn.ageTracker.AgeBiologicalTicks / TicksPerDay;
				return daysOld <= this._daysRange.max && daysOld >= this._daysRange.min;
			}

			long hoursOld = pawn.ageTracker.AgeBiologicalTicks / TicksPerHour;
			return hoursOld <= this._hoursRange.max && hoursOld >= this._hoursRange.min;
		}

		public override string Summary(PawnFilter filter) =>
			this._range.max > 0 ?
				this._range.min == this._range.max ? "PM.IsYearsOld".Translate(this._range.min)
				: "PM.IsBetweenYearsOld".Translate(this._range.min, this._range.max)
			: this._daysRange.max > 0 ?
				this._daysRange.min == this._daysRange.max ? "PM.IsDaysOld".Translate(this._daysRange.min)
				: "PM.IsBetweenDaysOld".Translate(this._daysRange.min, this._daysRange.max)
			: this._hoursRange.min == this._hoursRange.max ? "PM.IsHoursOld".Translate(this._hoursRange.min)
			: "PM.IsBetweenHoursOld".Translate(this._hoursRange.min, this._hoursRange.max);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			int displayedExtraRanges = this._range.max > 0 ? 0 : (this._daysRange.max > 0 ? 1 : 2);
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * displayedExtraRanges, out totalAddedListHeight, out Rect headerRemainderRect);

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this._range, MinAge, MaxAge);

			float labelWidthPercentage = 0.2f;
			if (displayedExtraRanges >= 1) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Rect daysRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
				Rect daysRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
				Widgets.Label(daysRect.LeftPart(labelWidthPercentage).Rounded(), "PM.Days".Translate().CapitalizeFirst());
				Widgets.IntRange(daysRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this._daysRange, 0, 60);

				if (displayedExtraRanges >= 2) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					Rect hoursRect = new Rect(rect.x, daysRect.yMax, rect.width, Text.LineHeight);
#else
					Rect hoursRect = new(rect.x, daysRect.yMax, rect.width, Text.LineHeight);
#endif
					Widgets.Label(hoursRect.LeftPart(labelWidthPercentage).Rounded(), "PM.Hours".Translate().CapitalizeFirst());
					Widgets.IntRange(hoursRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this._hoursRange, 0, 24);
				}
			}
		}

		internal override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(14, 30), Rand.Range(30, 100));
			this._daysRange = new IntRange(Rand.Range(0, 30), Rand.Range(30, 45));
			this._hoursRange = new IntRange(Rand.Range(0, 10), Rand.Range(10, 20));
#else
			this._range = new(Rand.Range(14, 30), Rand.Range(30, 100));
			this._daysRange = new(Rand.Range(0, 30), Rand.Range(30, 45));
			this._hoursRange = new(Rand.Range(0, 10), Rand.Range(10, 20));
#endif
		}
	}
}
