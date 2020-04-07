using HarmonyLib;
using Verse;

namespace PrepareModerately {
	[StaticConstructorOnStartup]
	class HarmonyPatcher { static HarmonyPatcher() => new Harmony("Lakuna.PrepareModerately").PatchAll(); }
}
