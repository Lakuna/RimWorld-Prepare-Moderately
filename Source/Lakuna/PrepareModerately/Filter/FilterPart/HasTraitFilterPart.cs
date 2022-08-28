using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasTraitFilterPart : FilterPart {
		public HasTraitDegreeData traitDegreeData;

		public override bool Matches(Pawn pawn) {
			return pawn.story.traits.allTraits.Find((Trait trait) => trait.def == this.traitDegreeData.trait)?.Degree == this.traitDegreeData.degree;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.traitDegreeData.Degree.LabelCap)) {
				FloatMenuUtility.MakeMenu(HasTraitDegreeData.AllTraitDegreeDatas,
					(HasTraitDegreeData traitDegreeData) => traitDegreeData.Degree.LabelCap,
					(HasTraitDegreeData traitDegreeData) => () => this.traitDegreeData = traitDegreeData);
			}
		}

		public override string Summary(Filter filter) {
			return "HasTrait".Translate(this.traitDegreeData.Degree.label);
		}

		public override void Randomize() {
			this.traitDegreeData = HasTraitDegreeData.AllTraitDegreeDatas[Rand.Range(0, HasTraitDegreeData.AllTraitDegreeDatas.Count - 1)];
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
