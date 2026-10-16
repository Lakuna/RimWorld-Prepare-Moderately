using System;

using Lakuna.PrepareModerately.Ui;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasNumberOfTraits : PawnFilterPart {
#pragma warning restore CA1812
#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private IntRange range;
#pragma warning restore IDE1006

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
		}

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			Widgets.IntRange(rect, Rand.Int, ref this.range, 1, 4);
		}

		internal override bool Matches(Pawn pawn) {
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
			return traitsCount >= this.range.min && traitsCount <= this.range.max;
		}

		internal override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.range = new IntRange(Rand.Range(1, 2), Rand.Range(2, 4));
#else
			this.range = new(Rand.Range(1, 2), Rand.Range(2, 4));
#endif

		internal override string Summary(PawnFilter filter) =>
			this.range.min == this.range.max ? "PM.HasNumberOfTraits".Translate(this.range.min)
			: "PM.HasBetweenNumberOfTraits".Translate(this.range.min, this.range.max);
	}
}
