using System;

using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	internal sealed class RollingDialog : Page {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Vector2 Size = new Vector2(450, 150);
#else
		private static readonly Vector2 Size = new(450, 150);
#endif

		public override Vector2 InitialSize =>
			Size;

		public override string PageTitle =>
			"PM.Rolling".Translate().CapitalizeFirst();

		private int iterations; // Will always be off by one (the "vanilla" iteration), but this should cause less confusion for the user.

		private readonly Action rollAction;

		internal RollingDialog(Action rollAction) {
			this.closeOnClickedOutside = true;
			this.rollAction = rollAction;
		}

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, $"{"PM.RollingNumber".Translate(this.iterations).CapitalizeFirst()}\n{"PM.ClickOutsideToStop".Translate().CapitalizeFirst()}");
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			for (int i = 0; i < PrepareModeratelyMod.Settings.PRollSpeedMultiplier; i++) {
				if (PawnFilter.Current.Matches(RandomizePatch.Result)) {
					this.Close();
					return;
				}

				this.rollAction();
				this.iterations++;
			}
		}

		public override void PreClose() {
			RandomizePatch.IsActivelyRolling = false;
			base.PreClose();
		}
	}
}
