using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace PrepareModerately {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "DoWindowContents")]
	class ButtonPatch {
		[HarmonyPostfix]
		public static void Postfix(Rect rect, Page_ConfigureStartingPawns __instance) {
			Vector2 buttonDimensions = new Vector2(150, 38);
			if (!Widgets.ButtonText(new Rect((rect.x + rect.width) / 2 - buttonDimensions.x / 2, rect.y - 45, buttonDimensions.x, buttonDimensions.y), "Prepare Moderately")) { return; }
			try {
				PrepareModerately.Instantiate(__instance);
				Find.WindowStack.Add(PrepareModerately.instance.page);
			} catch (Exception e) { Log.Warning("Failed to instantiate Prepare Moderately.\n" + e.StackTrace); }
		}
	}
}
