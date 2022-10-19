using Harmony;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[StaticConstructorOnStartup]
	public class HarmonyPatcher {
		static HarmonyPatcher() => HarmonyInstance.Create(nameof(PrepareModerately)).PatchAll(); // TODO: Harmony [2,)
	}
}
