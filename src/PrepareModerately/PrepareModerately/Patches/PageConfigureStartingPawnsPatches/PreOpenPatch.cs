#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using RimWorld;

namespace Lakuna.PrepareModerately.Patches.PageConfigureStartingPawnsPatches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.PreOpen))]
	internal static class PreOpenPatch {
		internal static Page_ConfigureStartingPawns _instance;

		[HarmonyPostfix]
#pragma warning disable CA1707, IDE1006 // Underscores are required for special Harmony parameters.
		private static void Postfix(Page_ConfigureStartingPawns __instance) =>
#pragma warning restore CA1707, IDE1006
			_instance = __instance;
	}
}
