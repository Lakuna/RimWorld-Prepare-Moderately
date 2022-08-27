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

			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(0.2f).Rounded(), "Count".Translate(this.count));
			this.count = (int)Widgets.HorizontalSlider(countRect.RightPart(1 - 0.2f), this.count, 1, 12);

			Rect levelRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(0.2f).Rounded(), "Level".Translate(this.level));
			this.level = (int)Widgets.HorizontalSlider(levelRect.RightPart(1 - 0.2f), this.level, 1, 20);
		}

		public override string Summary(Filter filter) {
			return "HasSkillsAtLevel".Translate(this.count, this.level);
		}

		public override void Randomize() {
			this.count = Rand.RangeInclusive(3, 6);
			this.level = Rand.RangeInclusive(5, 10);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.level, nameof(this.level));
			Scribe_Values.Look(ref this.count, nameof(this.count));
		}
	}
}
