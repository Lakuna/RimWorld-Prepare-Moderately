using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Patches;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class RollingDialog : Page {
		private int iterations;

		private readonly Action rollAction;

		private static readonly Vector2 size = new Vector2(450, 150);

		public RollingDialog(Action rollAction) {
			this.closeOnClickedOutside = true;
			this.rollAction = rollAction;
		}

		public override Vector2 InitialSize => size;

		public override string PageTitle => "Rolling".Translate().CapitalizeFirst();

		public override void DoWindowContents(Rect inRect) {
			this.iterations++;

			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "RollingNumber".Translate(this.iterations).CapitalizeFirst() + "\n" + "ClickOutsideToStop".Translate().CapitalizeFirst());
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			if (PawnFilter.Current.Matches(RandomizePatch.LastRandomizedPawn)) {
				this.Close();
				return;
			}

			this.rollAction();
		}

		public override void PreClose() {
			RandomizePatch.ActivelyRolling = false;
			base.PreClose();
		}
	}
}
