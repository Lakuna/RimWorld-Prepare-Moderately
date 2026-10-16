#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[StaticConstructorOnStartup]
	internal static class HarmonyPatcher {
#if V1_0
		private static readonly HarmonyInstance _instance = HarmonyInstance.Create(nameof(PrepareModerately));
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Harmony _instance = new Harmony(nameof(PrepareModerately));
#else
		private static readonly Harmony _instance = new(nameof(PrepareModerately));
#endif

		static HarmonyPatcher() =>
			_instance.PatchAll();
	}
}
