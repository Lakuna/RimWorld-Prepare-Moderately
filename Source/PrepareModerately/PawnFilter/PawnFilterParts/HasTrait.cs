using PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class HasTrait : PawnFilterPart {
		private TraitDef trait;
		public int degree;

		// For serialization.
		public string TraitLabelCap {
			get => this.trait.ToString().CapitalizeFirst();
			set => this.trait = DefDatabase<TraitDef>.AllDefsListForReading.Find((def) => def.ToString().CapitalizeFirst() == value);
		}

		public HasTrait() {
			this.trait = TraitDefOf.Cannibal;
			this.degree = this.trait.degreeDatas[0].degree;
		}

		public override bool Matches(Pawn pawn) => pawn.story.traits.allTraits.Find((trait) => trait.def == this.trait)?.Degree == this.degree;

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2);

			Rect traitRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(traitRect, this.TraitLabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<TraitDef>.AllDefsListForReading, (def) => def.ToString().CapitalizeFirst(), (def) => () => {
					this.trait = def;
					this.degree = this.trait.degreeDatas[0].degree;
				});
			}

			Rect degreeRect = new Rect(rect.x, traitRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(degreeRect, this.trait.DataAtDegree(this.degree).LabelCap)) {
				FloatMenuUtility.MakeMenu(this.trait.degreeDatas, (data) => this.trait.DataAtDegree(data.degree).LabelCap, (data) => () => this.degree = data.degree);
			}
		}
	}
}
