using PrepareModerately.GUI;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class InAgeRange : PawnFilterPart {
		[Serializable]
		public class InAgeRangeSerializable : PawnFilterPartSerializable {
			public int max;
			public int min;

			private InAgeRangeSerializable() { } // Parameterless constructor necessary for serialization.

			public InAgeRangeSerializable(InAgeRange pawnFilterPart) {
				this.max = pawnFilterPart.range.max;
				this.min = pawnFilterPart.range.min;
			}

			public override PawnFilterPart Deserialize() => new InAgeRange {
				range = new IntRange(this.min, this.max)
			};
		}

		public override PawnFilterPartSerializable Serialize() => new InAgeRangeSerializable(this);

		private static readonly System.Random random = new System.Random();
		protected IntRange range;

		public InAgeRange() {
			this.label = "In age range:";
			this.range = new IntRange(20, 55);
		}

		public override void DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			Widgets.IntRange(rect, random.Next(0x0, 0xFFFFF), ref this.range, 14);
		}

		public override bool Matches(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears <= this.range.max && pawn.ageTracker.AgeBiologicalYears >= this.range.min;
	}
}
