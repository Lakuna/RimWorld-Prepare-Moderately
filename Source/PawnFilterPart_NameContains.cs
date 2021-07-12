using System;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NameContains : PawnFilterPart {
		[Serializable]
		public class SerializableNameContains : SerializablePawnFilterPart {
			public string contains;

			public SerializableNameContains() { } // Parameterless constructor necessary for serialization.

			public SerializableNameContains(PawnFilterPart_NameContains pawnFilterPart) => this.contains = pawnFilterPart.contains;

			public override PawnFilterPart Deserialize() => new PawnFilterPart_NameContains {
				contains = this.contains
			};
		}


		public override SerializablePawnFilterPart Serialize() => new SerializableNameContains(this);
		private string contains;

		public PawnFilterPart_NameContains() {
			this.label = "Name contains:";
			this.contains = "Tynan";
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			this.contains = Widgets.TextArea(rect, this.contains);
		}

		public override bool Matches(Pawn pawn) => pawn.Name.ToStringFull.Contains(this.contains);
	}
}
