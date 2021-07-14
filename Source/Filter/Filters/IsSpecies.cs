using PrepareModerately.GUI;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class IsSpecies : PawnFilterPart {
		[Serializable]
		public class IsSpeciesSerializable : PawnFilterPartSerializable {
			public string humanlike;

			private IsSpeciesSerializable() { } // Parameterless constructor necessary for serialization.

			public IsSpeciesSerializable(IsSpecies pawnFilterPart) => this.humanlike = pawnFilterPart.humanlike.LabelCap;

			public override PawnFilterPart Deserialize() => new IsSpecies {
				humanlike = PawnFilter.AllHumanlikeDefs.Find(def => def.LabelCap == this.humanlike)
			};
		}

		public override PawnFilterPartSerializable Serialize() => new IsSpeciesSerializable(this);

		private ThingDef humanlike;

		public IsSpecies() {
			this.label = "Is species:";
			this.humanlike = ThingDefOf.Human;
		}

		public override float DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);

			if (Widgets.ButtonText(rect, this.humanlike.LabelCap)) {
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (ThingDef def in PawnFilter.AllHumanlikeDefs) { options.Add(new FloatMenuOption(def.LabelCap, () => this.humanlike = def)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}

			return RowHeight;
		}

		public override bool Matches(Pawn pawn) => pawn.def == this.humanlike;
	}
}
