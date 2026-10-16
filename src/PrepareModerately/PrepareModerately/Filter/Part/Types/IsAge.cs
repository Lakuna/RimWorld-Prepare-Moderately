using System;

using Lakuna.PrepareModerately.Ui;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsAge : PawnFilterPart {
#pragma warning restore CA1812
		private const int _maxAge = 120;

#if V1_0 || V1_1 || V1_2 || V1_3
		private const int _minAge = 14;
#else
		private const int _minAge = 0;
#endif

		private const int _ticksPerDay = 60000;

		private const int _ticksPerHour = 2500;

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private IntRange daysRange;

		private IntRange hoursRange;

		private IntRange range;
#pragma warning restore IDE1006

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Values.Look(ref this.daysRange, nameof(this.daysRange));
			Scribe_Values.Look(ref this.hoursRange, nameof(this.hoursRange));
		}

		internal override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			if (this.range.max > 0) {
				long yearsOld = pawn.ageTracker.AgeBiologicalYears;
				return yearsOld <= this.range.max && yearsOld >= this.range.min;
			}

			if (this.daysRange.max > 0) {
				long daysOld = pawn.ageTracker.AgeBiologicalTicks / _ticksPerDay;
				return daysOld <= this.daysRange.max && daysOld >= this.daysRange.min;
			}

			long hoursOld = pawn.ageTracker.AgeBiologicalTicks / _ticksPerHour;
			return hoursOld <= this.hoursRange.max && hoursOld >= this.hoursRange.min;
		}

		internal override string Summary(PawnFilter filter) =>
			this.range.max > 0 ?
				this.range.min == this.range.max ? "PM.IsYearsOld".Translate(this.range.min)
				: "PM.IsBetweenYearsOld".Translate(this.range.min, this.range.max)
			: this.daysRange.max > 0 ?
				this.daysRange.min == this.daysRange.max ? "PM.IsDaysOld".Translate(this.daysRange.min)
				: "PM.IsBetweenDaysOld".Translate(this.daysRange.min, this.daysRange.max)
			: this.hoursRange.min == this.hoursRange.max ? "PM.IsHoursOld".Translate(this.hoursRange.min)
			: "PM.IsBetweenHoursOld".Translate(this.hoursRange.min, this.hoursRange.max);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			int displayedExtraRanges = this.range.max > 0 ? 0 : (this.daysRange.max > 0 ? 1 : 2);
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * displayedExtraRanges, out totalAddedListHeight, out Rect headerRemainderRect);

			Widgets.IntRange(headerRemainderRect, Rand.Int, ref this.range, _minAge, _maxAge);

			float labelWidthPercentage = 0.2f;
			if (displayedExtraRanges >= 1) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Rect daysRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
				Rect daysRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
				Widgets.Label(daysRect.LeftPart(labelWidthPercentage).Rounded(), "PM.Days".Translate().CapitalizeFirst());
				Widgets.IntRange(daysRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.daysRange, 0, 60);

				if (displayedExtraRanges >= 2) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					Rect hoursRect = new Rect(rect.x, daysRect.yMax, rect.width, Text.LineHeight);
#else
					Rect hoursRect = new(rect.x, daysRect.yMax, rect.width, Text.LineHeight);
#endif
					Widgets.Label(hoursRect.LeftPart(labelWidthPercentage).Rounded(), "PM.Hours".Translate().CapitalizeFirst());
					Widgets.IntRange(hoursRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.hoursRange, 0, 24);
				}
			}
		}

		internal override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.range = new IntRange(Rand.Range(14, 30), Rand.Range(30, 100));
			this.daysRange = new IntRange(Rand.Range(0, 30), Rand.Range(30, 45));
			this.hoursRange = new IntRange(Rand.Range(0, 10), Rand.Range(10, 20));
#else
			this.range = new(Rand.Range(14, 30), Rand.Range(30, 100));
			this.daysRange = new(Rand.Range(0, 30), Rand.Range(30, 45));
			this.hoursRange = new(Rand.Range(0, 10), Rand.Range(10, 20));
#endif
		}
	}
}
