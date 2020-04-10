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
		private List<Pawn> pawnsBeforeRandomize;

		public static PrepareModerately Instance {
			get {
				if (PrepareModerately.instance == null) { PrepareModerately.instance = new PrepareModerately(); }
				return PrepareModerately.instance;
			}
		}

		/*
		public Pawn CurrentPawn {
			get {
				if (this.currentPawn == null) {
					if (Find.GameInitData.startingAndOptionalPawns.Count < 1) { Log.Warning("Failed to get current pawn (no starting or optional pawns)."); return null; }
					this.currentPawn = Find.GameInitData.startingAndOptionalPawns[0];
				}
				return this.currentPawn;
			}
			set => this.currentPawn = value;
		}
		*/

		public List<Pawn> PawnsBeforeRandomize { set => this.pawnsBeforeRandomize = value; }

		public Pawn JustRandomizedPawn {
			get {
				List<Pawn> currentPawns = Find.GameInitData.startingAndOptionalPawns;
				if (this.pawnsBeforeRandomize.Count != currentPawns.Count) { Log.Warning("Pawn count changed."); }
				List<Pawn> newPawns = new List<Pawn>();
				foreach (Pawn pawn in this.pawnsBeforeRandomize) {
					if (!currentPawns.Contains(pawn)) { newPawns.Add(pawn); }
				}
				if (newPawns.Count > 1) { Log.Warning("More than one pawn was randomized."); }
				if (newPawns.Count < 1) { Log.Warning("No pawn was randomized."); return null; }
				return newPawns[0];
			}
		}

		private PrepareModerately() {
			this.page = new Page_PrepareModerately();
			this.pawnsBeforeRandomize = Find.GameInitData.startingAndOptionalPawns;
			// this.currentFilter...
		}
	}
}

// StartingPawnUtility.RandomizeInPlace(currentPawn);
