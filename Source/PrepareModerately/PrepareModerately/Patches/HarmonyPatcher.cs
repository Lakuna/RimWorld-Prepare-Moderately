#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[StaticConstructorOnStartup]
	public class HarmonyPatcher {
		static HarmonyPatcher() =>
#if V1_0
			HarmonyInstance.Create(nameof(PrepareModerately)).PatchAll();
#else
			new Harmony(nameof(PrepareModerately)).PatchAll();
#endif
	}
}
