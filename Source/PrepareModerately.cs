using RimWorld;
using System;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately {
		private static PrepareModerately instance;
		public Page_PrepareModerately page;
		public Page_ConfigureStartingPawns originalPage;
		public PawnFilter currentFilter;
		public bool currentlyRandomizing;
		public Pawn currentPawn;

		public int RandomizeMultiplier => Math.Max(this.page.randomizeMultiplier, 1);

		public int RandomizeModulus => Math.Max(this.page.randomizeModulus, 1);

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
