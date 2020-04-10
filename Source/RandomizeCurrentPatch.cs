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
			PrepareModerately.Instance.PawnsBeforeRandomize = Find.GameInitData.startingAndOptionalPawns;
		}

		[HarmonyPostfix]
		public static void Postfix(Page_ConfigureStartingPawns __instance) {
			PrepareModerately.Instance.originalPage = __instance;
			Log.Message("Randomized. New pawn: " + PrepareModerately.Instance.JustRandomizedPawn + ".");
		}

		/*
		[HarmonyReversePatch]
		public static void ReversePatch() => throw new NotImplementedException("This is a stub and is not meant to be implemented.");
		*/
	}
}
