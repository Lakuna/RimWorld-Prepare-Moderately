using RimWorld;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately {
		private static PrepareModerately instance;
		public Page_PrepareModerately page;
		public Page_ConfigureStartingPawns originalPage;
		public PawnFilter currentFilter;

		public static PrepareModerately Instance {
			get {
				if (PrepareModerately.instance == null) { PrepareModerately.instance = new PrepareModerately(); }
				return PrepareModerately.instance;
			}
		}

		private PrepareModerately() {
			Log.Message("Instantiating Prepare Moderately.");
			this.page = new Page_PrepareModerately();
			this.currentFilter = new PawnFilter();
		}
	}
}
