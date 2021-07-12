using System;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_CapableOfEverything : PawnFilterPart {
		[Serializable]
		public class SerializableCapableOfEverything : SerializablePawnFilterPart {
			public SerializableCapableOfEverything() { } // Parameterless constructor necessary for serialization.

			public SerializableCapableOfEverything(PawnFilterPart_CapableOfEverything pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new PawnFilterPart_CapableOfEverything();
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableCapableOfEverything(this);

		public PawnFilterPart_CapableOfEverything() => this.label = "Capable of everything.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}

		public override bool Matches(Pawn pawn) {
			foreach (WorkTypeDef def in PawnFilter.allWorkTypes) {
				if (pawn.WorkTypeIsDisabled(def)) { return false; }
			}
			return true;
		}
	}
}
