using System;

using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	public sealed class ExceptionDialog : Window {
		private readonly Exception _e;

		public ExceptionDialog(Exception e) {
			this._e = e;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
		}

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "PM.PrepareModeratelyEncounteredAnException".TranslateFull(CapitalizeType.First, this._e.Message));
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.
		}
	}
}
