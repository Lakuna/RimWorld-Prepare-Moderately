using Harmony;
using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using System.Linq;
using System.Reflection;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	public static class RandomizePatch {
		public static bool ActivelyRolling { get; set; }

		public static Pawn LastRandomizedPawn { get; set; }

		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
#pragma warning restore CA1707
			LastRandomizedPawn = ___curPawn;

			if (PawnFilter.Current == null || !PawnFilter.Current.Parts.Any()) { return; }
			if (PawnFilter.Current.Matches(RandomizePatch.LastRandomizedPawn)) { return; }
			if (ActivelyRolling) { return; }

			Find.WindowStack.Add(new RollingDialog(delegate { __originalMethod.Invoke(__instance, null); }));

			ActivelyRolling = true;
		}
	}
}
