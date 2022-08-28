using System;
using Lakuna.PrepareModerately.Patches;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class RollingDialog : Page {
		private int iterations;

		private Action rollAction;

		public RollingDialog(Action rollAction) {
			this.closeOnClickedOutside = true;
			this.rollAction = rollAction;
		}

		public override Vector2 InitialSize => new Vector2(450, 150);

		public override string PageTitle => "Rolling".Translate().CapitalizeFirst();

		public override void DoWindowContents(Rect rect) {
			this.iterations++;

			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, "Rolling".Translate(this.iterations).CapitalizeFirst() + "\n" + "ClickOutsideToStop".Translate().CapitalizeFirst());
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			if (Filter.Filter.currentFilter.Matches(RandomizePatch.lastRandomizedPawn)) {
				this.Close();
				return;
			}

			this.rollAction();
		}

		public override void PreClose() {
			RandomizePatch.activelyRolling = false;
			base.PreClose();
		}
	}
}
