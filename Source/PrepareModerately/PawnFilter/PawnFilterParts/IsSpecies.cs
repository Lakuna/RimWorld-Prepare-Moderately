using PrepareModerately.UI;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class IsSpecies : PawnFilterPart {
		private static readonly List<ThingDef> humanlikeDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where((def) => def.race != null && def.race.Humanlike).ToList();

		private ThingDef species;

		// For serialization.
		public string SpeciesLabelCap {
			get => this.species.ToString().CapitalizeFirst();
			set => this.species = IsSpecies.humanlikeDefs.Find((def) => def.ToString().CapitalizeFirst() == value);
		}

		public IsSpecies() => this.species = ThingDefOf.Human;

		public override bool Matches(Pawn pawn) => pawn.def == this.species;

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.SpeciesLabelCap)) {
				FloatMenuUtility.MakeMenu(humanlikeDefs, (def) => def.ToString().CapitalizeFirst(), (def) => () => this.species = def);
			}
		}
	}
}
