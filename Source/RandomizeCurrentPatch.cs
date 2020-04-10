using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace PrepareModerately {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	class RandomizeCurrentPatch {
		[HarmonyPostfix]
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
			if (!PrepareModerately.Instance.currentFilter.Matches(___curPawn)) { _ = __originalMethod.Invoke(__instance, null); }
		}
	}
}
