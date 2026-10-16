using System;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	internal sealed class ExceptionDialog : Window {
		private readonly Exception _e;

		internal ExceptionDialog(Exception e) {
			this._e = e;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
		}

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "PM.PrepareModeratelyEncounteredAnException".Translate(this._e.Message).CapitalizeFirst());
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.
		}
	}
}
