using HarmonyLib;
using Verse;

namespace PrepareModerately.Patches {
	// Enables Harmony patches.
	[StaticConstructorOnStartup]
	public class HarmonyPatcher { static HarmonyPatcher() => new Harmony("Lakuna.PrepareModerately").PatchAll(); }
}
