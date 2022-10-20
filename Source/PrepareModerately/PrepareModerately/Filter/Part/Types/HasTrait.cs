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
			: pawn.story.traits.allTraits.Find((Trait trait) => trait.def == this.traitDegreePair.Trait)?.Degree == this.traitDegreePair.Degree;

		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
#if V1_0
			if (Widgets.ButtonText(rect, this.traitDegreePair.TraitDegreeData.label.CapitalizeFirst())) {
#else
			if (Widgets.ButtonText(rect, this.traitDegreePair.TraitDegreeData.LabelCap)) {
#endif
				FloatMenuUtility.MakeMenu(TraitDegreePair.TraitDegreePairs,
#if V1_0
					(TraitDegreePair traitDegreePair) => traitDegreePair.TraitDegreeData.label.CapitalizeFirst(),
#else
					(TraitDegreePair traitDegreePair) => traitDegreePair.TraitDegreeData.LabelCap,
#endif
					(TraitDegreePair traitDegreePair) => () => this.traitDegreePair = traitDegreePair);
			}
		}

		public override string Summary(PawnFilter filter) => "HasTrait".Translate(this.traitDegreePair.TraitDegreeData.label);

		public override void Randomize() {
			this.traitDegreePair = TraitDegreePair.TraitDegreePairs.ElementAt(Rand.Range(0, TraitDegreePair.TraitDegreePairs.Count() - 1));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this.traitDegreePair, nameof(this.traitDegreePair));
		}
	}
}
