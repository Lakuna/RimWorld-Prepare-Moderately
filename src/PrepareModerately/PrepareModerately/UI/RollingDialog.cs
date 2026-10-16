using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches.StartingPawnUtilityPatches;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	internal sealed class RollingDialog : Page {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Vector2 _size = new Vector2(450, 150);
#else
		private static readonly Vector2 _size = new(450, 150);
#endif

		public override Vector2 InitialSize =>
			_size;

		public override string PageTitle =>
			"PM.Rolling".Translate().CapitalizeFirst();

		private int _iterations; // Will always be off by one (the "vanilla" iteration), but this should cause less confusion for the user.

		private readonly Action _rollAction;

		internal RollingDialog(Action rollAction) {
			this.closeOnClickedOutside = true;
			this._rollAction = rollAction;
		}

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, $"{"PM.RollingNumber".Translate(this._iterations).CapitalizeFirst()}\n{"PM.ClickOutsideToStop".Translate().CapitalizeFirst()}");
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			for (int i = 0; i < PrepareModeratelyMod._settings.rollSpeedMultiplier; i++) {
				if (PawnFilter._current.Matches(RandomizeInPlacePatch._result)) {
					this.Close();
					return;
				}

				this._rollAction();
				this._iterations++;
			}
		}

		public override void PreClose() {
			RandomizeInPlacePatch._isActivelyRolling = false;
			base.PreClose();
		}
	}
}
