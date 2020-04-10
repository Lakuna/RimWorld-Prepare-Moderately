using Verse;

namespace PrepareModerately {
	public abstract class PawnFilterPart : IExposable {
		public string label;
		public PawnFilterPartDef def;

		public abstract bool Matches(Pawn pawn);

		public virtual void DoEditInterface(Listing_PawnFilter list) => _ = list.GetPawnFilterPartRect(this, Text.LineHeight);

		public void ExposeData() => Scribe_Defs.Look<PawnFilterPartDef>(ref this.def, "def");
	}
}
