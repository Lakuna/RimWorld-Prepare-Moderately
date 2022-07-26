using HarmonyLib;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[StaticConstructorOnStartup]
	public static class HarmonyPatcher {
		static HarmonyPatcher() {
			new Harmony("Lakuna.PrepareModerately").PatchAll();
		}
	}
}
