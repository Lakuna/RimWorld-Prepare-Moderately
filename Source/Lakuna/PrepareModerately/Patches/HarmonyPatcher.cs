using System.Reflection;
using HarmonyLib;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[StaticConstructorOnStartup]
	public class HarmonyPatcher {
		static HarmonyPatcher() {
			new Harmony(nameof(PrepareModerately)).PatchAll();
		}
	}
}
