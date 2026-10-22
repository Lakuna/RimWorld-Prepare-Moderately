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
	public static class DoWindowContentsPatch {
		private const float _buttonHeight = 30;

		private const float _buttonWidth = 150;

		private const float _buttonY = 45;

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
#pragma warning restore IDE1006
			if (Widgets.ButtonText(
				new Rect((rect.x + rect.width) / 2 - _buttonWidth / 2, rect.y - _buttonY, _buttonWidth, _buttonHeight).Rounded(),
#if V1_0
				"PM.PrepareModerately".Translate().CapitalizeAsTitleSafe())) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				"PM.PrepareModerately".Translate().StringSafe().CapitalizeAsTitleSafe())) {
#else
				"PM.PrepareModerately".Translate().StringSafe.CapitalizeAsTitleSafe)) {
#endif
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					SelectPawnFilterPage page = new SelectPawnFilterPage() {
#else
					SelectPawnFilterPage page = new() {
#endif
						prev = __instance,
						next = __instance
					};

					if (Find.WindowStack is null) {
						PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
						return;
					}

					Find.WindowStack.Add(page);
				} catch (Exception e) {
					SoundDefOf.ClickReject?.PlayOneShotOnCamera();
					if (Find.WindowStack is null) {
						PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
					} else {
						Find.WindowStack.Add(new ExceptionDialog(e));
					}
					PrepareModeratelyLogger.LogException(e, "Failed to initialize.");
					throw;
				}
			}
		}
	}
}
