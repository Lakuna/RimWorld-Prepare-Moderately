using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasTraitFilterPart : FilterPart {
		public TraitDef trait;

		private int degree;

		public int Degree {
			get {
				if (this.trait.DataAtDegree(this.degree) == null) {
					this.degree = this.trait.degreeDatas[0].degree;
				}
				return this.degree;
			}
			set {
				if (this.trait.DataAtDegree(value) == null) {
					this.degree = this.trait.degreeDatas[0].degree;
				} else {
					this.degree = value;
				}
			}
		}

		public override bool Matches(Pawn pawn) {
			return pawn.story.traits.allTraits.Find((Trait trait) => trait.def == this.trait)?.Degree == this.Degree;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight * 2);

			Rect traitRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(traitRect, this.trait.ToString())) {
				FloatMenuUtility.MakeMenu(DefDatabase<TraitDef>.AllDefsListForReading,
					(TraitDef def) => def.ToString(),
					(TraitDef def) => () => this.trait = def);
			}

			Rect degreeRect = new Rect(rect.x, traitRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(degreeRect, this.trait.DataAtDegree(this.Degree).label)) {
				FloatMenuUtility.MakeMenu(this.trait.degreeDatas,
					(TraitDegreeData degree) => degree.label,
					(TraitDegreeData degree) => () => this.Degree = degree.degree);
			}
		}

		public override string Summary(Filter filter) {
			return "HasTrait".Translate(this.trait.DataAtDegree(this.Degree).label);
		}

		public override void Randomize() {
			this.trait = DefDatabase<TraitDef>.AllDefsListForReading[Rand.RangeInclusive(0, DefDatabase<TraitDef>.AllDefsListForReading.Count - 1)];

			this.Degree = this.trait.degreeDatas[Rand.RangeInclusive(0, this.trait.degreeDatas.Count - 1)].degree;
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.trait, nameof(this.trait));
			Scribe_Values.Look(ref this.degree, nameof(this.degree));
		}
	}
}
