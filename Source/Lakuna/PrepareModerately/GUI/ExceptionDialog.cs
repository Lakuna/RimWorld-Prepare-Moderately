using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.GUI {
	public class ExceptionDialog : Window {
		private readonly Exception e;

		public ExceptionDialog(Exception e) {
			this.e = e;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
		}

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, String.Format("EncounteredAnException".Translate(), this.e.Message));
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.
		}
	}
}
