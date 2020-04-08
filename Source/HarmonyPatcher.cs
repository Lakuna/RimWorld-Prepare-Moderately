using HarmonyLib;
using Verse;

namespace PrepareModerately {
	[StaticConstructorOnStartup]
	public class HarmonyPatcher { static HarmonyPatcher() => new Harmony("Lakuna.PrepareModerately").PatchAll(); }
}
