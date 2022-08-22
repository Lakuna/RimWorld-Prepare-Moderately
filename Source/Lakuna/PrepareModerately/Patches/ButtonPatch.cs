using System;
using HarmonyLib;
using Lakuna.PrepareModerately.GUI;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
	public class ButtonPatch {
		[HarmonyPostfix]
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
			Vector2 buttonSize = new Vector2(150, 38);
			int buttonY = 45;
			Vector2 buttonPos = new Vector2((rect.x + rect.width) / 2 - buttonSize.x / 2, rect.y - buttonY);
			Rect buttonRect = new Rect(buttonPos.x, buttonPos.y, buttonSize.x, buttonSize.y);

			string buttonText = "PrepareModerately".Translate();

			if (Widgets.ButtonText(buttonRect, buttonText)) {
				try {
					throw new NotImplementedException(); // TODO
				} catch (Exception e) {
					SoundDefOf.ClickReject.PlayOneShotOnCamera();
					Find.WindowStack.Add(new ExceptionDialog(e));
					Logger.LogException(e, "Failed to initialize.");
				}
			}
		}
	}
}
