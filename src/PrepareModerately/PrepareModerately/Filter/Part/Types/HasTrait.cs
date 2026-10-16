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
		private static IEnumerable<TraitDegreePair> LegalTraitDegreePairs =>
			TraitDegreePair.TraitDegreePairs;

		private TraitDegreePair _traitDegreePair;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this._traitDegreePair, MiscUtil.NoUnder(nameof(this._traitDegreePair)));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2 || V1_3
			: pawn.story.traits.allTraits.Find((trait) => trait.def == this._traitDegreePair.Trait)?.Degree == this._traitDegreePair.Degree;
#else
			: pawn.story.traits.allTraits.Find((trait) => trait.def == this._traitDegreePair.Trait)?.Degree == this._traitDegreePair.Degree
				|| pawn.genes.GenesListForReading.Any((gene) => gene.def.suppressedTraits?.Any((traitData) => traitData.def == this._traitDegreePair.Trait && traitData.degree == this._traitDegreePair.Degree) ?? false);
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		public override bool NotMatches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.story.traits.allTraits.Find((trait) => trait.def == this._traitDegreePair.Trait)?.Degree != this._traitDegreePair.Degree
				|| pawn.genes.GenesListForReading.Any((gene) => gene.def.forcedTraits?.Any((traitData) => traitData.def == this._traitDegreePair.Trait && traitData.degree == this._traitDegreePair.Degree) ?? false);
#endif

		public override string Summary(PawnFilter filter) =>
			"PM.HasTrait".Translate(this._traitDegreePair.TraitDegreeData.label);

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
			if (Widgets.ButtonText(rect, this._traitDegreePair.TraitDegreeData.label.CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalTraitDegreePairs.OrderBy((pair) => pair.TraitDegreeData.label),
					(traitDegreePair) => traitDegreePair.TraitDegreeData.label.CapitalizeFirst(),
					(traitDegreePair) => () => this._traitDegreePair = traitDegreePair);
			}
#else
			if (Widgets.ButtonText(rect, this._traitDegreePair.TraitDegreeData.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalTraitDegreePairs.OrderBy((pair) => pair.TraitDegreeData.label),
					(traitDegreePair) => traitDegreePair.TraitDegreeData.LabelCap,
					(traitDegreePair) => () => this._traitDegreePair = traitDegreePair);
			}
#endif
		}

		internal override void Randomize() =>
			this._traitDegreePair = LegalTraitDegreePairs.RandomElement();
	}
}
