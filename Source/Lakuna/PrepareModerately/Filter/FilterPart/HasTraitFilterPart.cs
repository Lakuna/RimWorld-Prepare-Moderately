using System.Collections.Generic;
using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasTraitFilterPart : FilterPart {
		private static readonly List<HasTraitDegreeData> traitDegreeDatas;

		static HasTraitFilterPart() {
			HasTraitFilterPart.traitDegreeDatas = new List<HasTraitDegreeData>();

			foreach (TraitDef trait in DefDatabase<TraitDef>.AllDefsListForReading) {
				foreach (TraitDegreeData degree in trait.degreeDatas) {
					HasTraitFilterPart.traitDegreeDatas.Add(new HasTraitDegreeData(trait, degree.degree));
				}
			}
		}

		public HasTraitDegreeData traitDegreeData;

		public override bool Matches(Pawn pawn) {
			return pawn.story.traits.allTraits.Find((Trait trait) => trait.def == this.traitDegreeData.trait)?.Degree == this.traitDegreeData.degree;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.traitDegreeData.Degree.LabelCap)) {
				FloatMenuUtility.MakeMenu(HasTraitFilterPart.traitDegreeDatas,
					(HasTraitDegreeData traitDegreeData) => traitDegreeData.Degree.LabelCap,
					(HasTraitDegreeData traitDegreeData) => () => this.traitDegreeData = traitDegreeData);
			}
		}

		public override string Summary(Filter filter) {
			return "HasTrait".Translate(this.traitDegreeData.Degree.label);
		}

		public override void Randomize() {
			// this.traitDegreeData = HasTraitFilterPart.traitDegreeDatas[Rand.Range(0, HasTraitFilterPart.traitDegreeDatas.Count() - 1)];
			TraitDef trait = FilterPart.GetRandomOfDef(new TraitDef());
			this.traitDegreeData = new HasTraitDegreeData(trait, trait.degreeDatas[0].degree);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this.traitDegreeData, nameof(this.traitDegreeData));
		}
	}
}

// TODO: Make incompatible with incompatible traits.
// TODO: Make incompatible with incompatible backstories.
// TODO: Make incompatible with disabled work types.
