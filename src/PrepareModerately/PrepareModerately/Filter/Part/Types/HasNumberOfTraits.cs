using System;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasNumberOfTraits : PawnFilterPart {
#pragma warning restore CA1812
		private IntRange _range;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, MiscUtil.NoUnder(nameof(this._range)));
		}

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int geneForcedTraitCount = 0;
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			foreach (Gene gene in pawn.genes.GenesListForReading) {
				geneForcedTraitCount += gene.def.forcedTraits?.Count ?? 0;
			}
#endif

			int traitsCount = pawn.story.traits.allTraits.Count - geneForcedTraitCount;
			return traitsCount >= this._range.min && traitsCount <= this._range.max;
		}

		public override string Summary(PawnFilter filter) =>
			this._range.min == this._range.max ? "PM.HasNumberOfTraits".Translate(this._range.min)
			: "PM.HasBetweenNumberOfTraits".Translate(this._range.min, this._range.max);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			Widgets.IntRange(rect, Rand.Int, ref this._range, 1, 4);
		}

		internal override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(1, 2), Rand.Range(2, 4));
#else
			this._range = new(Rand.Range(1, 2), Rand.Range(2, 4));
#endif
	}
}
