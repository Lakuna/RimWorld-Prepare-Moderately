#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using RimWorld;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.PreOpen))]
	public static class PagePatch {
		public static Page_ConfigureStartingPawns Instance { get; set; }

		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Page_ConfigureStartingPawns __instance) => Instance = __instance;
#pragma warning restore CA1707
	}
}
