using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.GUI {
	// Based on Edb.PrepareCarefully.DialogInitializationError from https://github.com/edbmods/EdBPrepareCarefully.
	public class Dialog_Exception : Window {
		private readonly Exception exception;

		public Dialog_Exception(Exception e) {
			this.exception = e;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
		}

		public override void DoWindowContents(Rect inRect) {
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "EncounteredAnException".Translate() + "\n" + this.exception.Message);
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.
		}
	}
}
