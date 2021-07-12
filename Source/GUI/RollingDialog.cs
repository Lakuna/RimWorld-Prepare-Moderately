using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PrepareModerately.GUI {
	// Shows the user that the program is working as intended.
	public class RollingDialog : Page {
		private int iterations;
		private readonly Page_ConfigureStartingPawns page;
		private readonly MethodBase randomizeMethod;

		public RollingDialog(Page_ConfigureStartingPawns page, MethodBase randomizeMethod) {
			this.closeOnClickedOutside = true;
			this.iterations = 0;
			this.page = page;
			this.randomizeMethod = randomizeMethod;
		}

		public override Vector2 InitialSize => new Vector2(450, 150);

		public override string PageTitle => "Rolling";

		public override void DoWindowContents(Rect rect) {
			// Increment.
			this.iterations++;

			// Display window information.
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, "Rolling (" + this.iterations + ")\nClick outside of this dialog to stop.");

			// Don't randomize on non-modulus frames.
			if (this.iterations % PrepareModerately.Instance.RandomizeModulus != 0) { return; }

			// Randomize pawn.
			for (int i = 0; i < PrepareModerately.Instance.RandomizeMultiplier; i++) {
				// Stop randomizing if pawn matches filter.
				if (PrepareModerately.Instance.currentFilter.Matches(PrepareModerately.Instance.currentPawn)) {
					this.Close();
					return;
				}

				_ = this.randomizeMethod.Invoke(this.page, null);
			}
		}

		public override void PreClose() {
			PrepareModerately.Instance.currentlyRandomizing = false;
			base.PreClose();
		}
	}
}
