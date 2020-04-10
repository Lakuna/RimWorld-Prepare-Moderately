using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately {
		private static PrepareModerately instance;
		public Page_PrepareModerately page;
		public Page_ConfigureStartingPawns originalPage;
		public PawnFilter currentFilter;
		private List<string> pawnNamesBeforeRandomize;

		public static PrepareModerately Instance {
			get {
				if (PrepareModerately.instance == null) { PrepareModerately.instance = new PrepareModerately(); }
				return PrepareModerately.instance;
			}
		}

		public Pawn JustRandomizedPawn {
			get {
				List<Pawn> currentPawns = Find.GameInitData.startingAndOptionalPawns;
				if (this.pawnNamesBeforeRandomize.Count != currentPawns.Count) { Log.Warning("Pawn count changed."); }
				List<Pawn> newPawns = new List<Pawn>();
				foreach (Pawn pawn in currentPawns) {
					if (!this.pawnNamesBeforeRandomize.Contains(pawn.Name.ToStringFull)) { newPawns.Add(pawn); }
				}
				if (newPawns.Count > 1) { Log.Warning("More than one pawn was randomized."); }
				if (newPawns.Count < 1) { Log.Warning("No pawn was randomized."); return null; }
				return newPawns[0];
			}
		}

		private PrepareModerately() {
			Log.Message("Instantiating Prepare Moderately.");
			this.page = new Page_PrepareModerately();
			this.currentFilter = new PawnFilter();
			this.SaveCurrentPawnNames();
		}

		public void SaveCurrentPawnNames() {
			if (this.pawnNamesBeforeRandomize == null) { this.pawnNamesBeforeRandomize = new List<string>(); }
			this.pawnNamesBeforeRandomize.Clear();
			foreach (Pawn pawn in Find.GameInitData.startingAndOptionalPawns) { this.pawnNamesBeforeRandomize.Add(pawn.Name.ToStringFull); }
		}
	}
}

// StartingPawnUtility.RandomizeInPlace(currentPawn);
