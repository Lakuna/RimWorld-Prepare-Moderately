using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class Dialog_Randomizing : Page {
		private int iterations;
		private int randomizedPawns;
		private readonly Page_ConfigureStartingPawns page;
		private readonly MethodBase randomizeMethod;

		public Dialog_Randomizing(Page_ConfigureStartingPawns page, MethodBase randomizeMethod) {
			this.closeOnClickedOutside = true;
			this.iterations = 0;
			this.randomizedPawns = 0;
			this.page = page;
			this.randomizeMethod = randomizeMethod;
		}

		public override Vector2 InitialSize => new Vector2(450, 150);

		public override string PageTitle => "Randomizing";

		public override void DoWindowContents(Rect rect) {
			// Increment.
			this.iterations++;

			// Display window information.
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, "Randomizing (" + this.iterations + " iterations, " + this.randomizedPawns + " pawns)");
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(rect, "Click outside of this dialog to stop rolling.");
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(rect, PrepareModerately.Instance.RandomizeMultiplier + "X | " + PrepareModerately.Instance.RandomizeModulus + "%");

			// Don't randomize on non-modulus frames.
			if (this.iterations % PrepareModerately.Instance.RandomizeModulus != 0) { return; }

			// Re-randomize pawn.
			for (int i = 0; i < PrepareModerately.Instance.RandomizeMultiplier; i++) {
				// Stop randomizing if pawn matches filter.
				if (PrepareModerately.Instance.currentFilter.Matches(PrepareModerately.Instance.currentPawn)) {
					this.Close();
					return;
				}

				_ = this.randomizeMethod.Invoke(this.page, null);
				this.randomizedPawns++;
			}
		}

		public override void PreClose() {
			PrepareModerately.Instance.currentlyRandomizing = false;
			base.PreClose();
		}
	}
}
