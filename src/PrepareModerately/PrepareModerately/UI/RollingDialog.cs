using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches.StartingPawnUtilityPatches;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.Ui {
	public sealed class RollingDialog : Page {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Vector2 _size = new Vector2(450, 150);
#else
		private static readonly Vector2 _size = new(450, 150);
#endif

		public override Vector2 InitialSize =>
			_size;

		public override string PageTitle =>
#if V1_0
			"PM.Rolling".Translate()?.CapitalizeFirstSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.Rolling".Translate().CapitalizeFirst().StringSafe();
#else
			"PM.Rolling".Translate().CapitalizeFirst().StringSafe;
#endif

		private int _iterations; // Will always be off by one (the "vanilla" iteration), but this should cause less confusion for the user.

		private readonly Action _rollAction;

		public RollingDialog(Action rollAction) {
			this.closeOnClickedOutside = true;
			this._rollAction = rollAction;
		}

		public override void DoWindowContents(Rect inRect) {
			if (PrepareModeratelyMod.Settings?.HighVisibilityMode ?? false) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Widgets.DrawBoxSolid(inRect, new Color(1, 1, 0));
#else
				Widgets.DrawBoxSolid(inRect, new(1, 1, 0));
#endif
			}

			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, $"{"PM.RollingNumber".Translate(this._iterations).CapitalizeFirst()}\n{"PM.ClickOutsideToStop".Translate().CapitalizeFirst()}");
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			if (RandomizeInPlacePatch.Result is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to check filter against null pawn.");
				this.Close();
				return;
			}

			for (int i = 0; i < (PrepareModeratelyMod.Settings?.RollSpeedMultiplier ?? 1); i++) {
				if (PawnFilter.Current?.Matches(RandomizeInPlacePatch.Result) ?? true) {
					if ((PrepareModeratelyMod.Settings?.HighVisibilityMode ?? false) && this._iterations > 1000) {
						SoundDefOf.Crunch?.PlayOneShotOnCamera();
					}

					this.Close();
					return;
				}

				this._rollAction();
				this._iterations++;
			}
		}

		public override void PreClose() {
			base.PreClose();
			RandomizeInPlacePatch.IsActivelyRolling = false;
		}
	}
}
