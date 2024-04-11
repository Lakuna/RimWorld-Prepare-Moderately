#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;
using System.Linq;
using System.Reflection;
using Verse;

namespace Lakuna.PrepareModerately.Patches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.RandomizeInPlace))]
	public static class RandomizePatch {
		public static bool IsActivelyRolling { get; set; }

		public static Pawn Result { get; set; }

		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Pawn __result, MethodBase __originalMethod) {
#pragma warning restore CA1707
			/*
			 * Prepare Moderately causes pawns to be randomized asynchronously, since it can take a long time and users will
			 * need visual feedback. As a result, the return value of `Verse.StartingPawnUtility.RandomizeInPlace` will not
			 * always be accurate when Prepare Moderately is installed - instead, it will represent the first randomized pawn
			 * (as it would in vanilla). If anything in Prepare Moderately will cause an incompatibility, it will be this.
			 * 
			 * If you are a mod developer and you would like to account for this inconsistency, you will need to wait until
			 * `RandomizePatch.IsActivelyRolling` is `false` and then read the value of `RandomizePatch.Result`.
			 */

			Result = __result;

			if (PawnFilter.Current == null || !PawnFilter.Current.Parts.Any()) { return; }
			if (PawnFilter.Current.Matches(Result)) { return; }
			if (IsActivelyRolling) { return; }

			Find.WindowStack.Add(new RollingDialog(delegate { Result = (Pawn)__originalMethod.Invoke(null, new object[] { Result }); }));

			IsActivelyRolling = true;
		}
	}
}
