using PrepareModerately.GUI;
using System;
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

		public override void DoEditInterface(PawnFilterListing list) => _ = list.GetPawnFilterPartRect(this, 0);

		public override bool Matches(Pawn pawn) {
			foreach (WorkTypeDef def in PawnFilter.allWorkTypes) {
				if (pawn.WorkTypeIsDisabled(def)) { return false; }
			}
			return true;
		}
	}
}
