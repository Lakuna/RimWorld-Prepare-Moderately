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
			if (PrepareModerately.Instance.activeFilter == null || PrepareModerately.Instance.activeFilter.parts.NullOrEmpty()) {
				return;
			}

			PrepareModerately.Instance.activePawn = ___curPawn;

			if (PrepareModerately.Instance.activeFilter.Matches(___curPawn)) {
				return;
			}

			if (PrepareModerately.Instance.activelyRolling) {
				return;
			}

			Find.WindowStack.Add(new Dialog_Rolling(__instance, __originalMethod));
			PrepareModerately.Instance.activelyRolling = true;
		}
	}
}
