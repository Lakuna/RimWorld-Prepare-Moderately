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
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "RandomizeCurPawn")]
	class RandomizeCurrentPatch {
		[HarmonyPrefix]
		public static void Prefix(Page_ConfigureStartingPawns __instance) {
			PrepareModerately.Instance.originalPage = __instance;
			PrepareModerately.Instance.SaveCurrentPawnNames();
		}

		[HarmonyPostfix]
		public static void Postfix() {
			Log.Message("New pawn: " + PrepareModerately.Instance.JustRandomizedPawn);
			// TODO: Re-randomize until fits.
		}

		/*
		[HarmonyReversePatch]
		public static void ReversePatch() => throw new NotImplementedException("This is a stub and is not meant to be implemented.");
		*/
	}
}
