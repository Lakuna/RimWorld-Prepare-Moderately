#if V1_4 || V1_5 || V1_6
using System.Collections.Generic;
#endif

using Lakuna.PrepareModerately.Ui;
#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasNumberOfTraits : PawnFilterPart {
		private IntRange _range;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			Widgets.IntRange(rect, Rand.Int, ref this._range, 1, 4, "PM.RangeTraits");
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, "range");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._range.GetHashCode();

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3
			int traitsCount = pawn.story?.traits?.allTraits?.Count ?? 0;
#elif V1_4 || V1_5 || V1_6
			int geneForcedTraitCount = 0;
			foreach (Gene gene in pawn.genes?.GenesListForReading ?? new List<Gene>()) {
				geneForcedTraitCount += gene.def?.forcedTraits?.Count ?? 0;
			}

			int traitsCount = (pawn.story?.traits?.allTraits?.Count ?? 0) - geneForcedTraitCount;
#else
			int geneForcedTraitCount = 0;
			foreach (Gene gene in pawn.genes?.GenesListForReading ?? []) {
				geneForcedTraitCount += gene.def?.forcedTraits?.Count ?? 0;
			}

			int traitsCount = (pawn.story?.traits?.allTraits?.Count ?? 0) - geneForcedTraitCount;
#endif
			return traitsCount >= this._range.min && traitsCount <= this._range.max;
		}

		public override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(1, 2), Rand.Range(2, 4));
#else
			this._range = new(Rand.Range(1, 2), Rand.Range(2, 4));
#endif

		public override string Summary(PawnFilter filter) =>
			(this._range.min == this._range.max ? "PM.HasNumberOfTraits".Translate(this._range.min)
				: "PM.HasBetweenNumberOfTraits".Translate(this._range.min, this._range.max))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
