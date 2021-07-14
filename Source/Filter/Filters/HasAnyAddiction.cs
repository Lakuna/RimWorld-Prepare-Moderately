using PrepareModerately.GUI;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasAnyAddiction : PawnFilterPart {
		[Serializable]
		public class HasAnyAddictionSerializable : PawnFilterPartSerializable {
			public int workTag;

			private HasAnyAddictionSerializable() { } // Parameterless constructor necessary for serialization.

			public HasAnyAddictionSerializable(HasAnyAddiction pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new HasAnyAddiction();
		}

		public override PawnFilterPartSerializable Serialize() => new HasAnyAddictionSerializable(this);

		public HasAnyAddiction() => this.label = "Has any addiction.";

		public override float DoEditInterface(PawnFilterListing list) => list.GetPawnFilterPartRect(this, 0).height;

		public override bool Matches(Pawn pawn) => pawn.health.hediffSet.hediffs.Any(hediff => hediff.def.IsAddiction);
	}
}
