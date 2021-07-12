using System;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoRelationships : PawnFilterPart {
		[Serializable]
		public class SerializableNoRelationships : SerializablePawnFilterPart {
			public int workTag;

			public SerializableNoRelationships() { } // Parameterless constructor necessary for serialization.

			public SerializableNoRelationships(PawnFilterPart_NoRelationships pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new PawnFilterPart_NoRelationships();
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableNoRelationships(this);

		public PawnFilterPart_NoRelationships() => this.label = "No relationships.";

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}

		public override bool Matches(Pawn pawn) => !pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
	}
}
