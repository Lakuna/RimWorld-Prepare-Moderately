using System;
using Verse;

namespace PrepareModerately {
	public abstract class PawnFilterPart : IExposable {
		[Serializable]
		public abstract class SerializablePawnFilterPart {
			public abstract PawnFilterPart Deserialize();
		}

		public string label = "No label";
		public bool toRemove = false;
		public PawnFilterPartDef def;

		public static float RowHeight => Text.LineHeight;

		public abstract bool Matches(Pawn pawn);

		public abstract void DoEditInterface(Listing_PawnFilter list);

		public void ExposeData() => Scribe_Defs.Look(ref this.def, "def");

		public abstract SerializablePawnFilterPart Serialize();
	}
}
