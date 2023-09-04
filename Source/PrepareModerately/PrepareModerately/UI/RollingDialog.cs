using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class RollingDialog : Page {
		private int iterations; // Will always be off by one (the "vanilla" iteration), but this should cause less confusion for the user.

		private readonly Action rollAction;

		private static readonly Vector2 Size = new Vector2(450, 150);

		public RollingDialog(Action rollAction) {
			this.closeOnClickedOutside = true;
			this.rollAction = rollAction;
		}

		public override Vector2 InitialSize => Size;

		public override string PageTitle => "Rolling".Translate().CapitalizeFirst();

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "RollingNumber".Translate(this.iterations).CapitalizeFirst() + "\n" + "ClickOutsideToStop".Translate().CapitalizeFirst());
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			for (int i = 0; i < PrepareModeratelyMod.Settings.RollSpeedMultiplier; i++) {
				if (PawnFilter.Current.Matches(RandomizePatch.LastRandomizedPawn)) {
					this.Close();
					return;
				}

				this.rollAction();
				this.iterations++;
			}
		}

		public override void PreClose() {
			RandomizePatch.ActivelyRolling = false;
			base.PreClose();
		}
	}
}
