using RimWorld;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately {
		private static PrepareModerately instance;
		public Page_PrepareModerately page;
		public Page_ConfigureStartingPawns originalPage;
		public PawnFilter currentFilter;
		public bool currentlyRandomizing;
		public Pawn currentPawn;

		public static PrepareModerately Instance {
			get {
				if (instance == null) { instance = new PrepareModerately(); }
				return instance;
			}
		}

		private PrepareModerately() {
			this.page = new Page_PrepareModerately();
			this.currentFilter = new PawnFilter();
			this.currentlyRandomizing = false;
		}
	}
}
