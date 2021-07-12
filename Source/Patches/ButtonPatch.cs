using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Patches {
	// Adds the Prepare Moderately button to the configure starting pawns page.
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "DoWindowContents")]
	public class ButtonPatch {
		[HarmonyPostfix]
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
			PrepareModerately.Instance.originalPage = __instance;
			Vector2 buttonDimensions = new Vector2(150, 38); // Equivalent to the dimensions of the Prepare Carefully button.
			if (Widgets.ButtonText(new Rect((rect.x + rect.width) / 2 - buttonDimensions.x / 2, rect.y - 45, buttonDimensions.x, buttonDimensions.y), "Prepare Moderately")) {
				try {
					Find.WindowStack.Add(PrepareModerately.Instance.page);
					__instance.Close();
				} catch (Exception e) {
					Log.Warning("Failed to make window for Prepare Moderately (unexpected exception).\n" + e.StackTrace);
				}
			}
		}
	}
}
