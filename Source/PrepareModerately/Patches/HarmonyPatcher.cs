using HarmonyLib;
using Verse;

namespace PrepareModerately.Patches {
	[StaticConstructorOnStartup]
	public class HarmonyPatcher {
		static HarmonyPatcher() => new Harmony("Lakuna.PrepareModerately").PatchAll();
	}
}
