using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PrepareModerately.UI {
	public class Dialog_Rolling : Page {
		private const string title = "Rolling";

		private static Vector2 size = new Vector2(450, 150);

		private int iterations;
		private readonly Page_ConfigureStartingPawns configureStartingPawnsPage;
		private readonly MethodBase randomizeMethod;

		public Dialog_Rolling(Page_ConfigureStartingPawns configureStartingPawnsPage, MethodBase randomizeMethod) {
			this.closeOnClickedOutside = true;

			this.iterations = 0;
			this.configureStartingPawnsPage = configureStartingPawnsPage;
			this.randomizeMethod = randomizeMethod;
		}

		public override Vector2 InitialSize => Dialog_Rolling.size;

		public override string PageTitle => Dialog_Rolling.title;

		public override void DoWindowContents(Rect rect) {
			this.iterations++;

			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, "Rolling (" + this.iterations + ")\nClick outside of this dialog to stop.");
			Text.Anchor = TextAnchor.UpperLeft; // Must end on upper left.

			if (this.iterations % PrepareModerately.Instance.settings.rollModulus != 0) {
				return;
			}

			for (int i = 0; i < PrepareModerately.Instance.settings.rollMultiplier; i++) {
				if (PrepareModerately.Instance.activeFilter.Matches(PrepareModerately.Instance.activePawn)) {
					this.Close();
					return;
				}

				this.randomizeMethod.Invoke(this.configureStartingPawnsPage, null);
			}
		}

		public override void PreClose() {
			PrepareModerately.Instance.activelyRolling = false;
			base.PreClose();
		}
	}
}
