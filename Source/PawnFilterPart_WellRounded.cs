using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_WellRounded : PawnFilterPart {
		[Serializable]
		public class SerializableWellRounded : SerializablePawnFilterPart {
			public int skillCount;
			public int skillLevel;

			public SerializableWellRounded() { } // Parameterless constructor necessary for serialization.

			public SerializableWellRounded(PawnFilterPart_WellRounded pawnFilterPart) {
				this.skillCount = pawnFilterPart.skillCount;
				this.skillLevel = pawnFilterPart.skillLevel;
			}

			public override PawnFilterPart Deserialize() => new PawnFilterPart_WellRounded {
				skillCount = this.skillCount,
				skillLevel = this.skillLevel
			};
		}

		public override SerializablePawnFilterPart Serialize() => new SerializableWellRounded(this);

		private int skillCount;
		private string skillCountBuffer;
		private int skillLevel;
		private string skillLevelBuffer;

		public PawnFilterPart_WellRounded() {
			this.label = "Minimum skills at level:";
			this.skillCount = 2;
			this.skillLevel = 6;
		}

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * 2);

			// Skill count input field.
			Rect skillCountRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric(skillCountRect, ref this.skillCount, ref this.skillCountBuffer);

			// Skill level input field.
			Rect skillLevelRect = new Rect(rect.x, rect.y + skillCountRect.height, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric(skillLevelRect, ref this.skillLevel, ref this.skillLevelBuffer);
		}

		public override bool Matches(Pawn pawn) {
			int skills = 0;
			foreach (SkillRecord skill in pawn.skills.skills) {
				if (skill.levelInt >= this.skillLevel) { skills++; }
			}
			return skills > this.skillCount;
		}
	}
}
