using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasTrait : PawnFilterPart {
		private TraitDegreePair traitDegreePair;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.story.traits.allTraits.Find((trait) =>
				trait.def == this.traitDegreePair.Trait)?.Degree == this.traitDegreePair.Degree
#if V1_0 || V1_1 || V1_2 || V1_3
			;
#else
			|| pawn.genes.GenesListForReading.Any((gene) =>
				gene.def.suppressedTraits?.Any((traitData) =>
					traitData.def == this.traitDegreePair.Trait
					&& traitData.degree == this.traitDegreePair.Degree
				) ?? false
			);
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		// Override NOT gate functionality to disregard the gene override.
		public override bool NotMatches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.story.traits.allTraits.Find((trait) =>
				trait.def == this.traitDegreePair.Trait)?.Degree != this.traitDegreePair.Degree
			|| pawn.genes.GenesListForReading.Any((gene) =>
				gene.def.forcedTraits?.Any((traitData) =>
					traitData.def == this.traitDegreePair.Trait
					&& traitData.degree == this.traitDegreePair.Degree
				) ?? false
			);
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
#if V1_0
			if (Widgets.ButtonText(rect, this.traitDegreePair.TraitDegreeData.label.CapitalizeFirst())) {
				IOrderedEnumerable<TraitDegreePair> traits = TraitDegreePair.TraitDegreePairs
					.OrderBy((pair) => pair.TraitDegreeData.label);

				FloatMenuUtility.MakeMenu(traits,
					(traitDegreePair) => traitDegreePair.TraitDegreeData.label.CapitalizeFirst(),
					(traitDegreePair) => () => this.traitDegreePair = traitDegreePair);
			}
#else
			if (Widgets.ButtonText(rect, this.traitDegreePair.TraitDegreeData.LabelCap)) {
				IOrderedEnumerable<TraitDegreePair> traits = TraitDegreePair.TraitDegreePairs.OrderBy((pair) => pair.TraitDegreeData.label);
				FloatMenuUtility.MakeMenu(traits,
					(traitDegreePair) => traitDegreePair.TraitDegreeData.LabelCap,
					(traitDegreePair) => () => this.traitDegreePair = traitDegreePair);
			}
#endif
		}

		public override string Summary(PawnFilter filter) => "HasTrait".Translate(this.traitDegreePair.TraitDegreeData.label);

		public override void Randomize() => this.traitDegreePair = TraitDegreePair.TraitDegreePairs
			.ElementAt(Rand.Range(0, TraitDegreePair.TraitDegreePairs.Count() - 1));

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this.traitDegreePair, nameof(this.traitDegreePair));
		}
	}
}
