using System;
using HarmonyLib;
using Lakuna.PrepareModerately.GUI;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Patches {
	// Based on EdB.PrepareCarefully.HarmonyPatches.PrepareCarefullyButtonPatch from https://github.com/edbmods/EdBPrepareCarefully.
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
	public static class ButtonPatch {
		[HarmonyPostfix]
		private static void Postfix(Rect inRect, Page_ConfigureStartingPawns __instance) {
			Vector2 buttonSize = new Vector2(150, 38);
			int buttonY = 45;
			Vector2 buttonPos = new Vector2((inRect.x + inRect.width) / 2 - buttonSize.x / 2, inRect.y - buttonY);
			Rect buttonRect = new Rect(buttonPos.x, buttonPos.y, buttonSize.x, buttonSize.y);

			string buttonText = "PrepareModerately".Translate();

			if (Widgets.ButtonText(buttonRect, buttonText)) {
				try {
					PrepareModeratelyMod.page_ConfigureStartingPawns = __instance;
					Find.WindowStack.Add(new Page_SelectFilter());
				} catch (Exception e) {
					SoundDefOf.ClickReject.PlayOneShot(new SoundInfo());
					Find.WindowStack.Add(new Dialog_Exception(e));
					Logger.LogException(e, "failed to initialize");
				}
			}
		}
	}
}
