using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoAddictions : PawnFilterPart {
		[Serializable]
		public class SerializableNoAddictions : SerializablePawnFilterPart {
			public int workTag;

			public SerializableNoAddictions(PawnFilterPart_NoAddictions pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new PawnFilterPart_NoAddictions();
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableNoAddictions(this);

		public PawnFilterPart_NoAddictions() => this.label = "No addictions.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}
		public override bool Matches(Pawn pawn) => !pawn.health.hediffSet.hediffs.Any(hediff => hediff.def.IsAddiction);
	}
}
