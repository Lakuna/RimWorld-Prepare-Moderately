using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace PrepareModerately {
	/*
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "SelectPawn")]
	class SelectPawnPatch {
		[HarmonyPostfix]
		public static void Postfix(Pawn c, Page_ConfigureStartingPawns __instance) {
			PrepareModerately.Instance.originalPage = __instance;
			PrepareModerately.Instance.CurrentPawn = c;
			Log.Message("Selected new pawn.");
			// This is not ever being called.
		}
	}
	*/
}
