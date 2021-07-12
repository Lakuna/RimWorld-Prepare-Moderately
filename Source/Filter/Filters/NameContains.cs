using PrepareModerately.GUI;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class NameContains : PawnFilterPart {
		[Serializable]
		public class NameContainsSerializable : PawnFilterPartSerializable {
			public string contains;

			private NameContainsSerializable() { } // Parameterless constructor necessary for serialization.

			public NameContainsSerializable(NameContains pawnFilterPart) => this.contains = pawnFilterPart.contains;

			public override PawnFilterPart Deserialize() => new NameContains {
				contains = this.contains
			};
		}


		public override PawnFilterPartSerializable Serialize() => new NameContainsSerializable(this);
		private string contains;

		public NameContains() {
			this.label = "Name contains:";
			this.contains = "Tynan";
		}

		public override void DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			this.contains = Widgets.TextArea(rect, this.contains);
		}

		public override bool Matches(Pawn pawn) => pawn.Name.ToStringFull.Contains(this.contains);
	}
}
