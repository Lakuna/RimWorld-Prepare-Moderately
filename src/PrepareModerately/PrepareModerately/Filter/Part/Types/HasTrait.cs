using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasTrait : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<TraitDegree> _legalTraitDegreePairs =>
			TraitDegree._all;

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private TraitDegree traitDegreePair;
#pragma warning restore IDE1006

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this.traitDegreePair, nameof(this.traitDegreePair));
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
#if V1_0
			if (Widgets.ButtonText(rect, this.traitDegreePair._data.label.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(_legalTraitDegreePairs.OrderBy((pair) => pair._data.label),
					(traitDegreePair) => traitDegreePair._data.label.CapitalizeFirst(),
					(traitDegreePair) => () => this.traitDegreePair = traitDegreePair);
			}
#else
			if (Widgets.ButtonText(rect, this.traitDegreePair._data.LabelCap)) {
				FloatMenuUtility.MakeMenu(_legalTraitDegreePairs.OrderBy((pair) => pair._data.label),
					(traitDegreePair) => traitDegreePair._data.LabelCap,
					(traitDegreePair) => () => this.traitDegreePair = traitDegreePair);
			}
#endif
		}

		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.story.traits.allTraits.Find((trait) => trait.def == this.traitDegreePair.trait)?.Degree == this.traitDegreePair.degree;
#else
			: pawn.story.traits.allTraits.Find((trait) => trait.def == this.traitDegreePair.trait)?.Degree == this.traitDegreePair.degree
				|| pawn.genes.GenesListForReading.Any((gene) => gene.def.suppressedTraits?.Any((traitData) => traitData.def == this.traitDegreePair.trait && traitData.degree == this.traitDegreePair.degree) ?? false);
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		internal override bool NotMatches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.story.traits.allTraits.Find((trait) => trait.def == this.traitDegreePair.trait)?.Degree != this.traitDegreePair.degree
				|| pawn.genes.GenesListForReading.Any((gene) => gene.def.forcedTraits?.Any((traitData) => traitData.def == this.traitDegreePair.trait && traitData.degree == this.traitDegreePair.degree) ?? false);
#endif

		internal override void Randomize() =>
			this.traitDegreePair = _legalTraitDegreePairs.RandomElement();

		internal override string Summary(PawnFilter filter) =>
			"PM.HasTrait".Translate(this.traitDegreePair._data.label);
	}
}
