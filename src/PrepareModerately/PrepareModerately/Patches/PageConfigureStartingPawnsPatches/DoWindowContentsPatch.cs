using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Patches.PageConfigureStartingPawnsPatches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
	internal static class DoWindowContentsPatch {
		private const float _buttonHeight = 30;

		private const float _buttonWidth = 150;

		private const float _buttonY = 45;

		[HarmonyPostfix]
#pragma warning disable CA1707, IDE1006 // Underscores are required for special Harmony parameters.
		private static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
#pragma warning restore CA1707, IDE1006
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(new Rect((rect.x + rect.width) / 2 - _buttonWidth / 2, rect.y - _buttonY, _buttonWidth, _buttonHeight), "PM.PrepareModerately".Translate().CapitalizeFirst())) {
#else
			if (Widgets.ButtonText(new((rect.x + rect.width) / 2 - _buttonWidth / 2, rect.y - _buttonY, _buttonWidth, _buttonHeight), "PM.PrepareModerately".Translate().CapitalizeFirst())) {
#endif
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					SelectPawnFilterPage page = new SelectPawnFilterPage {
						prev = __instance,
						next = __instance
					};
#else
					SelectPawnFilterPage page = new() {
						prev = __instance,
						next = __instance
					};
#endif
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
