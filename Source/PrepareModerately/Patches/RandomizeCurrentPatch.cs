using HarmonyLib;
using PrepareModerately.UI;
using RimWorld;
using System.Reflection;
using Verse;

namespace PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	public class RandomizeCurrentPatch {
		[HarmonyPostfix]
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
			if (PrepareModerately.page == null || PrepareModerately.page.filter.parts.NullOrEmpty()) {
				return;
			}

			PrepareModerately.activePawn = ___curPawn;

			if (PrepareModerately.page.filter.Matches(___curPawn)) {
				return;
			}

			if (PrepareModerately.activelyRolling) {
				return;
			}

			Find.WindowStack.Add(new Dialog_Rolling(__instance, __originalMethod));
			PrepareModerately.activelyRolling = true;
		}
	}
}
