using PrepareModerately.GUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class IsCapableOf : PawnFilterPart {
		[Serializable]
		public class IsCapableOfSerializable : PawnFilterPartSerializable {
			public int workTag;

			private IsCapableOfSerializable() { } // Parameterless constructor necessary for serialization.

			public IsCapableOfSerializable(IsCapableOf pawnFilterPart) => this.workTag = (int) pawnFilterPart.workTag;

			public override PawnFilterPart Deserialize() => new IsCapableOf {
				workTag = (WorkTags) this.workTag
			};
		}

		public override PawnFilterPartSerializable Serialize() => new IsCapableOfSerializable(this);

		private WorkTags workTag;

		public IsCapableOf() {
			this.label = "Is capable of:";
			this.workTag = WorkTags.Firefighting;
		}

		public override void DoEditInterface(PawnFilterListing list) {
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
