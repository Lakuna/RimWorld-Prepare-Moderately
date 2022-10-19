using Harmony;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	public static class RandomizePatch {
		private static bool activelyRolling;

		public static bool ActivelyRolling {
			get => RandomizePatch.activelyRolling;
			set => RandomizePatch.activelyRolling = value;
		}

		private static Pawn lastRandomizedPawn;

		public static Pawn LastRandomizedPawn {
			get => RandomizePatch.lastRandomizedPawn;
			set => RandomizePatch.lastRandomizedPawn = value;
		}

		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
#pragma warning restore CA1707
			RandomizePatch.LastRandomizedPawn = ___curPawn;

			/*
			 * TODO:
			 * if (Filter.Filter.currentFilter == null || Filter.Filter.currentFilter.AllParts.Count() == 0) { return; }
			 * if (Filter.Filter.currentFilter.Matches(RandomizePatch.lastRandomizedPawn)) { return; }
			 */
			if (RandomizePatch.ActivelyRolling) { return; }

			/*
			 * TODO:
			 * Find.WindowStack.Add(new RollingDialog(delegate { __originalMethod.Invoke(__instance, null); }));
			 */

			RandomizePatch.ActivelyRolling = true;
		}
	}
}
