using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasTraitFilterPart : FilterPart {
		public TraitDef trait;

		public TraitDegreeData degree;

		public override bool Matches(Pawn pawn) {
			return pawn.story.traits.allTraits.Find((Trait trait) => trait.def == this.trait).Degree == this.degree.degree;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight * 2);

			Rect traitRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(traitRect, this.trait.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(DefDatabase<TraitDef>.AllDefsListForReading,
					(TraitDef def) => def.ToString().CapitalizeFirst(),
					(TraitDef def) => () => {
						this.trait = def;
						this.degree = this.trait.degreeDatas[0];
					});
			}

			Rect degreeRect = new Rect(rect.x, traitRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(degreeRect, this.degree.LabelCap)) {
				FloatMenuUtility.MakeMenu(this.trait.degreeDatas,
					(TraitDegreeData degree) => this.trait.DataAtDegree(degree.degree).LabelCap,
					(TraitDegreeData degree) => () => this.degree = degree);
			}
		}

		public override string Summary(Filter filter) {
			return "HasTrait".Translate(this.degree.LabelCap);
		}

		public override void Randomize() {
			this.trait = DefDatabase<TraitDef>.AllDefsListForReading[Rand.RangeInclusive(0, DefDatabase<TraitDef>.AllDefsListForReading.Count - 1)];

			this.degree = this.trait.degreeDatas[Rand.RangeInclusive(0, this.trait.degreeDatas.Count - 1)];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
