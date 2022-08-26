using System.Linq;
using System.Reflection;
using HarmonyLib;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using Verse;

// TODO: Consider patching methods on the Verse.PawnGenerator class instead.

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	public class RandomizePatch {
		public static bool activelyRolling;

		public static Pawn lastRandomizedPawn;

		[HarmonyPostfix]
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
			RandomizePatch.lastRandomizedPawn = ___curPawn;

			if (Filter.Filter.currentFilter == null || Filter.Filter.currentFilter.AllParts.Count() == 0) { return; }
			if (Filter.Filter.currentFilter.Matches(RandomizePatch.lastRandomizedPawn)) { return; }
			if (RandomizePatch.activelyRolling) { return; }

			Find.WindowStack.Add(new RollingDialog(delegate {
				__originalMethod.Invoke(__instance, null);
			}));

			RandomizePatch.activelyRolling = true;
		}
	}
}
