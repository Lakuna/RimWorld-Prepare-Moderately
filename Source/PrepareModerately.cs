using PrepareModerately.Filter;
using PrepareModerately.GUI;
using RimWorld;
using System;
using System.IO;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately {
		public static string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Lakuna/PrepareModerately";
		private static PrepareModerately instance;
		public static PrepareModerately Instance {
			get {
				if (instance == null) { instance = new PrepareModerately(); }
				return instance;
			}
		}

		public PrepareModeratelyPage page;
		public Page_ConfigureStartingPawns originalPage;
		public PawnFilter currentFilter;
		public bool currentlyRandomizing;
		public Pawn currentPawn;
		public int RandomizeMultiplier => Math.Max(this.page.randomizeMultiplier, 1);
		public int RandomizeModulus => Math.Max(this.page.randomizeModulus, 1);

		private PrepareModerately() {
			_ = Directory.CreateDirectory(dataPath);
			this.page = new PrepareModeratelyPage();
			this.currentFilter = new PawnFilter();
			this.currentlyRandomizing = false;
		}
	}
}
