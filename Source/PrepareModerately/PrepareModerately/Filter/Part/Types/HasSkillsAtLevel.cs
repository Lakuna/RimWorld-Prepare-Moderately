using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasSkillsAtLevel : PawnFilterPart {
		private int count;

		private int level;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.skills.FindAll((skill) => skill.Level >= this.level).Count >= this.count;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

			float labelWidthPercentage = 0.2f;
			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(labelWidthPercentage).Rounded(), "CountNumber".Translate(this.count).CapitalizeFirst());
#if V1_0 || V1_1 || V1_2 || V1_3
			this.count = (int)Widgets.HorizontalSlider(countRect.RightPart(1 - labelWidthPercentage), this.count, 1, 12);
#else
			float count = this.count;
			Widgets.HorizontalSlider(countRect.RightPart(1 - labelWidthPercentage), ref count, new FloatRange(1, 12));
			this.count = (int)count;
#endif

			Rect levelRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "LevelNumber".Translate(this.level).CapitalizeFirst());
#if V1_0 || V1_1 || V1_2 || V1_3
			this.level = (int)Widgets.HorizontalSlider(levelRect.RightPart(1 - labelWidthPercentage), this.level, 1, 20);
#else
			float level = this.level;
			Widgets.HorizontalSlider(levelRect.RightPart(1 - labelWidthPercentage), ref level, new FloatRange(1, 20));
			this.level = (int)level;
#endif
		}

		public override string Summary(PawnFilter filter) => "HasSkillsAtLevel".Translate(this.count, this.level);

		public override void Randomize() {
			this.count = Rand.Range(3, 6);
			this.level = Rand.Range(5, 10);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.level, nameof(this.level));
			Scribe_Values.Look(ref this.count, nameof(this.count));
		}
	}
}
