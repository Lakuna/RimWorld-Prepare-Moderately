using PrepareModerately.GUI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.Filter.Filters {
	public class HasMinimumSkillsAtLevel : PawnFilterPart {
		[Serializable]
		public class HasMinimumSkillsAtLevelSerializable : PawnFilterPartSerializable {
			public int skillCount;
			public int skillLevel;

			private HasMinimumSkillsAtLevelSerializable() { } // Parameterless constructor necessary for serialization.

			public HasMinimumSkillsAtLevelSerializable(HasMinimumSkillsAtLevel pawnFilterPart) {
				this.skillCount = pawnFilterPart.skillCount;
				this.skillLevel = pawnFilterPart.skillLevel;
			}

			public override PawnFilterPart Deserialize() => new HasMinimumSkillsAtLevel {
				skillCount = this.skillCount,
				skillLevel = this.skillLevel
			};
		}

		public override PawnFilterPartSerializable Serialize() => new HasMinimumSkillsAtLevelSerializable(this);

		private int skillCount;
		private string skillCountBuffer;
		private int skillLevel;
		private string skillLevelBuffer;

		public HasMinimumSkillsAtLevel() {
			this.label = "Has minimum skills at level:";
			this.skillCount = 2;
			this.skillLevel = 6;
		}

		public override float DoEditInterface(PawnFilterListing list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight * 2);

			// Skill count input field.
			Rect skillCountRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric(skillCountRect, ref this.skillCount, ref this.skillCountBuffer);

			// Skill level input field.
			Rect skillLevelRect = new Rect(rect.x, rect.y + skillCountRect.height, rect.width, rect.height / 2);
			Widgets.TextFieldNumeric(skillLevelRect, ref this.skillLevel, ref this.skillLevelBuffer);

			return RowHeight * 2;
		}

		public override bool Matches(Pawn pawn) {
			int skills = 0;
			foreach (SkillRecord skill in pawn.skills.skills) {
				if (skill.levelInt >= this.skillLevel) { skills++; }
			}
			return skills >= this.skillCount;
		}
	}
}
