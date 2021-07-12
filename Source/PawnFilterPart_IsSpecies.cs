using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_IsSpecies : PawnFilterPart {
		[Serializable]
		public class SerializableIsSpecies : SerializablePawnFilterPart {
			public string humanlike;

			public SerializableIsSpecies(PawnFilterPart_IsSpecies pawnFilterPart) => this.humanlike = pawnFilterPart.humanlike.LabelCap;

			public override PawnFilterPart Deserialize() => new PawnFilterPart_IsSpecies {
				humanlike = PawnFilter.AllHumanlikeDefs.Find(def => def.LabelCap == this.humanlike)
			};
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableIsSpecies(this);

		private ThingDef humanlike;

		public PawnFilterPart_IsSpecies() {
			this.label = "Is species:";
			this.humanlike = ThingDefOf.Human;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			if (!Widgets.ButtonText(rect, this.humanlike.LabelCap)) { return; }

			// Fill humanlike dropdown when button is pressed.
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (ThingDef def in PawnFilter.AllHumanlikeDefs) { options.Add(new FloatMenuOption(def.LabelCap, () => this.humanlike = def)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) => pawn.def == this.humanlike;
	}
}
