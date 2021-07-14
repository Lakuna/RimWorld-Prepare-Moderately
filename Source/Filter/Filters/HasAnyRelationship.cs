using PrepareModerately.GUI;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasAnyRelationship : PawnFilterPart {
		[Serializable]
		public class HasAnyRelationshipSerializable : PawnFilterPartSerializable {
			public int workTag;

			private HasAnyRelationshipSerializable() { } // Parameterless constructor necessary for serialization.

			public HasAnyRelationshipSerializable(HasAnyRelationship pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new HasAnyRelationship();
		}

		public override PawnFilterPartSerializable Serialize() => new HasAnyRelationshipSerializable(this);

		public HasAnyRelationship() => this.label = "Has any relationship.";

		public override float DoEditInterface(PawnFilterListing list) => list.GetPawnFilterPartRect(this, 0).height;

		public override bool Matches(Pawn pawn) => pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
	}
}
