using PrepareModerately.GUI;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class IsCapableOfEverything : PawnFilterPart {
		[Serializable]
		public class IsCapableOfEverythingSerializable : PawnFilterPartSerializable {
			private IsCapableOfEverythingSerializable() { } // Parameterless constructor necessary for serialization.

			public IsCapableOfEverythingSerializable(IsCapableOfEverything pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new IsCapableOfEverything();
		}

		public override PawnFilterPartSerializable Serialize() => new IsCapableOfEverythingSerializable(this);

		public IsCapableOfEverything() => this.label = "Is capable of everything.";

		public override float DoEditInterface(PawnFilterListing list) => list.GetPawnFilterPartRect(this, 0).height;

		public override bool Matches(Pawn pawn) {
			foreach (WorkTypeDef def in PawnFilter.allWorkTypes) {
				if (pawn.WorkTypeIsDisabled(def)) { return false; }
			}
			return true;
		}
	}
}
