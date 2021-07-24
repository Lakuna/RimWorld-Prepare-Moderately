using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
	public class ButtonPatch {
		private const string buttonText = "Prepare Moderately";
		private const int buttonY = -45;

		private static Vector2 buttonDimensions = new Vector2(150, 38);

		[HarmonyPostfix]
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
			PrepareModerately.Instance.configureStartingPawnsPage = __instance;

			if (Widgets.ButtonText(new Rect((rect.x + rect.width) / 2 - ButtonPatch.buttonDimensions.x / 2, rect.y + buttonY,
				ButtonPatch.buttonDimensions.x, ButtonPatch.buttonDimensions.y), ButtonPatch.buttonText)) {
				try {
					Find.WindowStack.Add(PrepareModerately.Instance.Page);
					__instance.Close();
				} catch (Exception e) {
					PrepareModerately.LogError(e);
				}
			}
		}
	}
}
