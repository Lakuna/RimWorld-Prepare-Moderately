using PrepareModerately.GUI;
using System;
using System.Linq;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasAnyPermanentMedicalCondition : PawnFilterPart {
		[Serializable]
		public class HasAnyPermanentMedicalConditionSerializable : PawnFilterPartSerializable {
			public int workTag;

			private HasAnyPermanentMedicalConditionSerializable() { } // Parameterless constructor necessary for serialization.

			public HasAnyPermanentMedicalConditionSerializable(HasAnyPermanentMedicalCondition pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new HasAnyPermanentMedicalCondition();
		}

		public override PawnFilterPartSerializable Serialize() => new HasAnyPermanentMedicalConditionSerializable(this);

		public HasAnyPermanentMedicalCondition() => this.label = "Has any permanent medical condition.";

		public override void DoEditInterface(PawnFilterListing list) => _ = list.GetPawnFilterPartRect(this, 0);

		public override bool Matches(Pawn pawn) => pawn.health.hediffSet.hediffs.Any(hediff => hediff.IsPermanent() || hediff.def.chronic);
	}
}
