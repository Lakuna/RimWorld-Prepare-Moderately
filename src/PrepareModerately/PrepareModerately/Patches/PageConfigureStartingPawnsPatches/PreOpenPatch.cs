#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using RimWorld;

namespace Lakuna.PrepareModerately.Patches.PageConfigureStartingPawnsPatches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.PreOpen))]
	public static class PreOpenPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static Page_ConfigureStartingPawns Instance {
#else
		public static Page_ConfigureStartingPawns? Instance {
#endif
			get; set;
		}

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Page_ConfigureStartingPawns __instance) =>
#pragma warning restore IDE1006
			Instance = __instance;
	}
}
