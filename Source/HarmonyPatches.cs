using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace PrepareModerately {
	[StaticConstructorOnStartup]
	class HarmonyPatches {
		static HarmonyPatches() {
			try {
				Harmony harmony = new Harmony("Lakuna.PrepareModerately");

				Type configureStartingPawnsPageType = Type.GetType("RimWorld.Page_ConfigureStartingPawns");

				if (configureStartingPawnsPageType == null) { Log.Warning("Failed to add elements to the configure pawns page (type not found)."); return; }

				MethodBase preOpenMethod = configureStartingPawnsPageType.GetMethod("PreOpen");
				HarmonyMethod preOpenMethodPostfix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("PreOpenPostfix"));

				if (harmony.Patch(preOpenMethod, null, preOpenMethodPostfix) == null) { Log.Warning("Failed to patch the Page_ConfigureStartingPawns.PreOpen method."); }

				MethodBase doWindowContentsMethod = configureStartingPawnsPageType.GetMethod("DoWindowContents");
				HarmonyMethod doWindowContentsMethodPostfix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DoWindowContentsPostfix"));

				if (harmony.Patch(doWindowContentsMethod, null, doWindowContentsMethodPostfix) == null) { Log.Warning("Failed to patch the Page_ConfigureStartingPawns.DoWindowContentsPostfix method."); }
			} catch (Exception e) { Log.Warning("Failed to patch for Prepare Moderately (unexpected exception).\n" + e.StackTrace); }
		}

		public static void PreOpenPostfix() {

		}

		public static void DoWindowContentsPostfix(Rect rect, Page_ConfigureStartingPawns instance) {
			Vector2 position = new Vector2(150, 150);
			float y = rect.height + 45;
			if (!Widgets.ButtonText(new Rect((float) (rect.x + rect.width / 2.0 - position.x / 2.0), y, position.x, position.y), "Prepare Moderately")) { return; }
		}
	}
}
