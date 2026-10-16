using System.Linq;
using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches.PageConfigureStartingPawnsPatches;
using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.RandomizeInPlace))]
	internal static class RandomizeInPlacePatch {
		internal static bool IsActivelyRolling {
			get; set;
		}

		internal static Pawn Result {
			get; set;
		}

		[HarmonyPostfix]
#pragma warning disable CA1707, IDE1006 // Underscores are required for special Harmony parameters.
		private static void Postfix(Pawn __result, MethodBase __originalMethod) {
#pragma warning restore CA1707, IDE1006
			Result = __result;
			PreOpenPatch.Instance.SelectPawn(Result);
			if (IsActivelyRolling || PawnFilter.Current is null || !PawnFilter.Current.parts.Any() || PawnFilter.Current.Matches(Result)) {
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
