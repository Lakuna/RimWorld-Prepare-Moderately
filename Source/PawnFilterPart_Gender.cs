using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	internal class PawnFilterPart_Gender : PawnFilterPart {
		private Gender gender;

		public PawnFilterPart_Gender() {
			this.label = "Gender:";
			this.gender = Gender.Male;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			if (!Widgets.ButtonText(rect, this.gender.ToString())) { return; }

			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (Gender gender in Enum.GetValues(typeof(Gender))) { options.Add(new FloatMenuOption(gender.ToString(), () => this.gender = gender)); }
			Find.WindowStack.Add(new FloatMenu(options));
		}

		public override bool Matches(Pawn pawn) => pawn.gender == this.gender;

		public override string ToLoadableString() => this.GetType().Name + " " + this.gender.ToString();

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			foreach (Gender gender in Enum.GetValues(typeof(Gender))) {
				if (gender.ToString() == parts[1]) {
					this.gender = gender;
					break;
				}
			}
		}
	}
}
