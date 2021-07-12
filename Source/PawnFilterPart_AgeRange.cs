using System;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_AgeRange : PawnFilterPart {
		[Serializable]
		public class SerializableAgeRange : SerializablePawnFilterPart {
			public int max;
			public int min;

			public SerializableAgeRange(PawnFilterPart_AgeRange pawnFilterPart) {
				this.max = pawnFilterPart.range.max;
				this.min = pawnFilterPart.range.min;
			}

			public override PawnFilterPart Deserialize() => new PawnFilterPart_AgeRange {
				range = new IntRange(this.min, this.max)
			};
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableAgeRange(this);

		private static readonly System.Random random = new System.Random();
		protected IntRange range;

		public PawnFilterPart_AgeRange() {
			this.label = "Age range:";
			this.range = new IntRange(20, 55);
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.IntRange(rect, PawnFilterPart_AgeRange.random.Next(0x0, 0xFFFFF), ref this.range, 14);
		}

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears <= this.range.max && pawn.ageTracker.AgeBiologicalYears >= this.range.min;
	}
}
