using System;

using Lakuna.PrepareModerately.UI;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasNumberOfTraits : PawnFilterPart {
		private IntRange range;

		public override bool Matches(Pawn pawn) {
			if (pawn == null) {
				throw new ArgumentNullException(nameof(pawn));
			}

			int geneForcedTraitCount = 0;
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			foreach (Gene gene in pawn.genes.GenesListForReading) {
				geneForcedTraitCount += gene.def.forcedTraits?.Count ?? 0;
			}
#endif

			int traitsCount = pawn.story.traits.allTraits.Count - geneForcedTraitCount;
			return traitsCount >= this.range.min && traitsCount <= this.range.max;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			Widgets.IntRange(rect, Rand.Int, ref this.range, 1, 4);
		}

		public override string Summary(PawnFilter filter) => this.range.min == this.range.max
			? "PM.HasNumberOfTraits".Translate(this.range.min)
			: "PM.HasBetweenNumberOfTraits".Translate(this.range.min, this.range.max);

		public override void Randomize() => this.range = new IntRange(Rand.Range(1, 2), Rand.Range(2, 4));

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
		}
	}
}
