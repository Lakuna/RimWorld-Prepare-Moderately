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
			// if (!PrepareModerately.Instance.currentFilter.Matches(PrepareModerately.Instance.JustRandomizedPawn)) { _ = StartingPawnUtility.RandomizeInPlace(PrepareModerately.Instance.JustRandomizedPawn); }
			if (!PrepareModerately.Instance.JustRandomizedPawn.Name.ToStringFull.StartsWith("A")) { _ = StartingPawnUtility.RandomizeInPlace(PrepareModerately.Instance.JustRandomizedPawn); } // For testing until filters get finished.
																																															   // TODO: Index out of range exception. Try repeating with a reverse patch instead.
		}
	}
}
