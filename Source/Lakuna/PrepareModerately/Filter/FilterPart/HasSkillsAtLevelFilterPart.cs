using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasSkillsAtLevelFilterPart : FilterPart {
		public int count;

		public int level;

		public override bool Matches(Pawn pawn) {
			return pawn.skills.skills.FindAll((SkillRecord skill) => skill.Level >= this.level).Count >= this.count;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight * 2);

			float labelWidthPercentage = 0.2f;
			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(labelWidthPercentage).Rounded(), "Count".Translate(this.count).CapitalizeFirst());
			this.count = (int)Widgets.HorizontalSlider(countRect.RightPart(1 - labelWidthPercentage), this.count, 1, 12);

			Rect levelRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "Level".Translate(this.level).CapitalizeFirst());
			this.level = (int)Widgets.HorizontalSlider(levelRect.RightPart(1 - labelWidthPercentage), this.level, 1, 20);
		}

		public override string Summary(Filter filter) {
			return "HasSkillsAtLevel".Translate(this.count, this.level);
		}

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
