using PrepareModerately.GUI;
using System;
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

		public override void DoEditInterface(PawnFilterListing list) => _ = list.GetPawnFilterPartRect(this, 0);

		public override bool Matches(Pawn pawn) => pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
	}
}
