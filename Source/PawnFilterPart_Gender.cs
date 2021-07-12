using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_Gender : PawnFilterPart {
		[Serializable]
		public class SerializableGender : SerializablePawnFilterPart {
			public string gender;

			public SerializableGender() { } // Parameterless constructor necessary for serialization.

			public SerializableGender(PawnFilterPart_Gender pawnFilterPart) => this.gender = pawnFilterPart.gender.ToString();

			public override PawnFilterPart Deserialize() {
				foreach (Gender gender in Enum.GetValues(typeof(Gender))) {
					if (gender.ToString() == this.gender) {
						return new PawnFilterPart_Gender() { gender = gender };
					}
				}
				throw new Exception("Tried to load unknown gender \"" + this.gender + "\".");
			}
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableGender(this);

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
	}
}
