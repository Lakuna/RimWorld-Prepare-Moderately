using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace PrepareModerately {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	class RandomizeCurrentPatch {
		[HarmonyPostfix]
		public static void Postfix(Page_ConfigureStartingPawns __instance, MethodBase __originalMethod, Pawn ___curPawn) {
			Log.Message("New pawn: " + ___curPawn.Name.ToStringFull + ".");
			// if (!PrepareModerately.Instance.currentFilter.Matches(___curPawn)) { _ = __originalMethod.Invoke(__instance, null); }
			if (!___curPawn.Name.ToStringFull.StartsWith("A")) { _ = __originalMethod.Invoke(__instance, null); } // For testing until filters get finished.
		}
	}
}
