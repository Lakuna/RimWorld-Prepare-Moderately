using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class Dialog_Randomizing : Page {
		private int iterations;
		private readonly Page_ConfigureStartingPawns page;
		private readonly MethodBase randomizeMethod;

		public Dialog_Randomizing(Page_ConfigureStartingPawns page, MethodBase randomizeMethod) {
			this.closeOnClickedOutside = true;
			this.iterations = 0;
			this.page = page;
			this.randomizeMethod = randomizeMethod;
		}

		public override Vector2 InitialSize => new Vector2(300, 100);

		public override string PageTitle => "Randomizing";

		public override void DoWindowContents(Rect rect) {
			// Increment.
			this.iterations++;

			// Display window information.
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, "Randomizing (" + this.iterations + ")");
			Text.Anchor = TextAnchor.UpperLeft;

			// Stop randomizing if pawn matches filter.
			if (PrepareModerately.Instance.currentFilter.Matches(PrepareModerately.Instance.currentPawn)) {
				this.Close();
				return;
			}

			// Re-randomize pawn.
			_ = this.randomizeMethod.Invoke(this.page, null);
		}

		public override void PreClose() {
			PrepareModerately.Instance.currentlyRandomizing = false;
			base.PreClose();
		}
	}
}
