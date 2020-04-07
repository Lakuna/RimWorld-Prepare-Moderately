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
			Vector2 position = new Vector2(150, 150);
			float y = rect.height + 45;
			if (!Widgets.ButtonText(new Rect(rect.x + rect.width / 2.0f - position.x / 2.0f, y, position.x, position.y), "Prepare Moderately", true, false, true)) { return; }
			try {
				PrepareModerately.Instantiate(__instance);
				Find.WindowStack.Add(PrepareModerately.instance.page);
			} catch (Exception e) { Log.Warning("Failed to instantiate Prepare Moderately.\n" + e.StackTrace); }
		}
	}
}
