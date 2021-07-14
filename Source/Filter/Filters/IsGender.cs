using PrepareModerately.GUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class IsGender : PawnFilterPart {
		[Serializable]
		public class IsGenderSerializable : PawnFilterPartSerializable {
			public string gender;

			private IsGenderSerializable() { } // Parameterless constructor necessary for serialization.

			public IsGenderSerializable(IsGender pawnFilterPart) => this.gender = pawnFilterPart.gender.ToString();

			public override PawnFilterPart Deserialize() {
				foreach (Gender gender in Enum.GetValues(typeof(Gender))) {
					if (gender.ToString() == this.gender) {
						return new IsGender() { gender = gender };
					}
				}
				throw new Exception("Tried to load unknown gender \"" + this.gender + "\".");
			}
		}

		public override PawnFilterPartSerializable Serialize() => new IsGenderSerializable(this);

		private Gender gender;

		public IsGender() {
			this.label = "Is gender:";
			this.gender = Gender.Male;
		}

		public override float DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);

			if (Widgets.ButtonText(rect, this.gender.ToString())) {
				List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach (Gender gender in Enum.GetValues(typeof(Gender))) { options.Add(new FloatMenuOption(gender.ToString(), () => this.gender = gender)); }
				Find.WindowStack.Add(new FloatMenu(options));
			}

			return RowHeight;
		}

		public override bool Matches(Pawn pawn) => pawn.gender == this.gender;
	}
}
