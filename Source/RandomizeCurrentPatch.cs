using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace PrepareModerately {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	public class RandomizeCurrentPatch {
		[HarmonyPostfix]
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
			// Just skip everything if there is no filter.
			if (PrepareModerately.Instance.currentFilter.parts.Count < 1) { return; }

			// Update current pawn so that it's accessible elsewhere.
			PrepareModerately.Instance.currentPawn = ___curPawn;

			// Don't do complex stuff if the pawn already works.
			if (PrepareModerately.Instance.currentFilter.Matches(___curPawn)) { return; }

			// Ensure randomizing dialog doesn't get added twice.
			if (PrepareModerately.Instance.currentlyRandomizing) { return; }

			// Add randomizing dialog.
			Find.WindowStack.Add(new Dialog_Randomizing(__instance, __originalMethod));
			PrepareModerately.Instance.currentlyRandomizing = true;
		}
	}
}
