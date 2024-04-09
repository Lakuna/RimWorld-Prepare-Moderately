#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.UI;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
using RimWorld;
#endif
using System.Linq;
using System.Reflection;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
#else
	[HarmonyPatch(typeof(StartingPawnUtility), "RegenerateStartingPawnInPlace")]
#endif
	public static class RandomizePatch {
		public static bool IsActivelyRolling { get; set; }

		public static Pawn LastRandomizedPawn { get; set; }

		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
			LastRandomizedPawn = ___curPawn;
			void rollAction() => _ = __originalMethod.Invoke(__instance, null);
#else
		public static void Postfix(int index, MethodBase __originalMethod) {
			LastRandomizedPawn = Find.GameInitData.startingAndOptionalPawns[index];
			void rollAction() => _ = __originalMethod.Invoke(null, new object[] { index });
#endif
#pragma warning restore CA1707

			if (PawnFilter.Current == null || !PawnFilter.Current.Parts.Any()) { return; }
			if (PawnFilter.Current.Matches(LastRandomizedPawn)) { return; }
			if (IsActivelyRolling) { return; }

			Find.WindowStack.Add(new RollingDialog(rollAction));

			IsActivelyRolling = true;
		}
	}
}
