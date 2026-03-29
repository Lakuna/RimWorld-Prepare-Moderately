#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.UI;
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
			Result = __result;
			PagePatch.Instance.SelectPawn(Result);

			if (IsActivelyRolling) {
				return;
			}

			if (PawnFilter.Current == null || !PawnFilter.Current.Parts.Any()) {
				return;
			}

			if (PawnFilter.Current.Matches(Result)) {
				return;
			}

			Find.WindowStack.Add(new RollingDialog(() => _ = __originalMethod.Invoke(null, new object[] { Result })));

			IsActivelyRolling = true;
		}
	}
}
