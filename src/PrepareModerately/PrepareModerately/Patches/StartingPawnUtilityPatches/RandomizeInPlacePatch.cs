using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches.PageConfigureStartingPawnsPatches;
using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.RandomizeInPlace))]
	public static class RandomizeInPlacePatch {
		public static bool IsActivelyRolling {
			get; set;
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static Pawn Result {
#else
		public static Pawn? Result {
#endif
			get; set;
		}

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn __result, MethodBase __originalMethod) {
#pragma warning restore IDE1006
			Result = __result;

			if (PreOpenPatch.Instance is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to select pawn on null page.");
				return;
			}

			PreOpenPatch.Instance.SelectPawn(Result);
			if (IsActivelyRolling || PawnFilter.Current?.Parts is null || PawnFilter.Current.Parts.Count < 1 || PawnFilter.Current.Matches(Result)) {
				return;
			}

			if (Find.WindowStack is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to add window to null stack.");
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Find.WindowStack.Add(new RollingDialog(() => _ = __originalMethod.Invoke(null, new object[] { Result })));
#else
			Find.WindowStack.Add(new RollingDialog(() => _ = __originalMethod.Invoke(null, [Result])));
#endif

			IsActivelyRolling = true;
		}
	}
}
