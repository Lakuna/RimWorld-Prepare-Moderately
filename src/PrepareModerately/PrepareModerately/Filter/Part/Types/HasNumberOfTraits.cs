using System.Collections.Generic;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

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
			int geneForcedTraitCount = 0;
#if !(V1_0 || V1_1 || V1_2 || V1_3)
#if V1_4 || V1_5 || V1_6
			List<Gene> empty = new List<Gene>();
#else
			List<Gene> empty = [];
#endif
			foreach (Gene gene in pawn.genes?.GenesListForReading ?? empty) {
				geneForcedTraitCount += gene.def?.forcedTraits?.Count ?? 0;
			}
#endif
			int traitsCount = (pawn.story?.traits?.allTraits?.Count ?? 0) - geneForcedTraitCount;
			return traitsCount >= this._range.min && traitsCount <= this._range.max;
		}

		public override void Randomize() {
			int min = Rand.Range(1, 2);
			int max = Rand.Range(2, 4);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(min, max);
#else
			this._range = new(min, max);
#endif
		}

		public override string Summary(PawnFilter filter) =>
			(this._range.min == this._range.max) ? "PM.HasNumberOfTraits".TranslateFull(this._range.min)
			: "PM.HasBetweenNumberOfTraits".TranslateFull(this._range.min, this._range.max);
	}
}
