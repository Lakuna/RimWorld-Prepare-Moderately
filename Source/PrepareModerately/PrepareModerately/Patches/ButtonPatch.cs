#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
	public static class ButtonPatch {
		private static readonly Vector2 ButtonSize = new Vector2(150, 38);

		private const float ButtonY = 45;

		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
#pragma warning restore CA1707
			if (Widgets.ButtonText(new Rect((rect.x + rect.width) / 2 - ButtonSize.x / 2, rect.y - ButtonY, ButtonSize.x, ButtonSize.y), "PrepareModerately".Translate().CapitalizeFirst())) {
				try {
					SelectPawnFilterPage page = new SelectPawnFilterPage {
						// prev = __instance, // Disable "Back" button to prevent people from accidentally not setting filters.
						next = __instance
					};
					Find.WindowStack.Add(page);
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					SoundDefOf.ClickReject.PlayOneShotOnCamera();
					Find.WindowStack.Add(new ExceptionDialog(e));
					PrepareModeratelyLogger.LogException(e, "Failed to initialize.");
				}
			}
		}
	}
}
