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
		private Pawn currentPawn;

		public static PrepareModerately Instance {
			get {
				if (PrepareModerately.instance == null) { PrepareModerately.instance = new PrepareModerately(); }
				return PrepareModerately.instance;
			}
		}

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

		private PrepareModerately() {
			this.page = new Page_PrepareModerately();
			// this.currentFilter...
		}
	}
}

// StartingPawnUtility.RandomizeInPlace(currentPawn);
