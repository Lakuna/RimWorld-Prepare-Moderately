using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_CapableOf : PawnFilterPart {
		private WorkTags workTag;

		public PawnFilterPart_CapableOf() {
			this.label = "Capable of:";
			this.workTag = WorkTags.Firefighting;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);

			// Don't do anything when the button isn't clicked.
			if (!Widgets.ButtonText(rect, this.workTag.ToString().CapitalizeFirst())) { return; }

			// Fill dropdown.
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (WorkTags workTag in Enum.GetValues(typeof(WorkTags))) { options.Add(new FloatMenuOption(workTag.ToString().CapitalizeFirst(), () => this.workTag = workTag)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) => !pawn.WorkTagIsDisabled(this.workTag);
	}
}
