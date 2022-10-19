using Harmony;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
	public static class ButtonPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
#pragma warning restore CA1707
			Vector2 buttonSize = new Vector2(150, 38);
			int buttonY = 45;
			Vector2 buttonPos = new Vector2((rect.x + rect.width) / 2 - buttonSize.x / 2, rect.y - buttonY);
			Rect buttonRect = new Rect(buttonPos.x, buttonPos.y, buttonSize.x, buttonSize.y);

			string buttonText = "PrepareModerately".Translate().CapitalizeFirst();

			if (Widgets.ButtonText(buttonRect, buttonText)) {
				try {
					/*
					 * TODO:
					 * SelectFilterPage page = new SelectFilterPage();
					 * // page.prev = __instance; // Disable "Back" button to prevent people from accidentally not setting filters.
					 * page.next = __instance;
					 * Find.WindowStack.Add(page);
					 */
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					SoundDefOf.ClickReject.PlayOneShotOnCamera();
					// TODO: Find.WindowStack.Add(new ExceptionDialog(e));
					Logger.LogException(e, "Failed to initialize.");
				}
			}
		}
	}
}
