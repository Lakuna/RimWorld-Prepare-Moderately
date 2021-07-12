using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NoPermanentMedicalConditions : PawnFilterPart {
		[Serializable]
		public class SerializableNoPermanentMedicalConditions : SerializablePawnFilterPart {
			public int workTag;

			public SerializableNoPermanentMedicalConditions(PawnFilterPart_NoPermanentMedicalConditions pawnFilterPart) { }

			public override PawnFilterPart Deserialize() => new PawnFilterPart_NoPermanentMedicalConditions();
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableNoPermanentMedicalConditions(this);

		public PawnFilterPart_NoPermanentMedicalConditions() => this.label = "No permanent medical conditions.";
		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.Label(rect, "No input.");
		}
		public override bool Matches(Pawn pawn) => !pawn.health.hediffSet.hediffs.Any(hediff => hediff.IsPermanent() || hediff.def.chronic);
	}
}
