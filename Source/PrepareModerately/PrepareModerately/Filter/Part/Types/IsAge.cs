using Lakuna.PrepareModerately.UI;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsAge : PawnFilterPart {
		private IntRange range;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.ageTracker.AgeBiologicalYears <= this.range.max && pawn.ageTracker.AgeBiologicalYears >= this.range.min;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight, out totalAddedListHeight);
			Widgets.IntRange(rect, Rand.Int, ref this.range, 14, 120);
		}

		public override string Summary(PawnFilter filter) => "IsBetweenYearsOld".Translate(this.range.min, this.range.max);

		public override void Randomize() => this.range = new IntRange(Rand.Range(14, 30), Rand.Range(30, 100));

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
		}
	}
}
