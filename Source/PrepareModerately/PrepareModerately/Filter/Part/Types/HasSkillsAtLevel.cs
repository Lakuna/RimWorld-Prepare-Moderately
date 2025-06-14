using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasSkillsAtLevel : PawnFilterPart {
		private IntRange countRange;

		private IntRange range;

		public override bool Matches(Pawn pawn) {
			if (pawn == null) {
				throw new ArgumentNullException(nameof(pawn));
			}

			int count = pawn.skills.skills.FindAll((skill) => skill.Level <= this.range.max && skill.Level >= this.range.min).Count;
			return count <= this.countRange.max && count >= this.countRange.min;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

			float labelWidthPercentage = 0.2f;
			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(labelWidthPercentage).Rounded(), "Count".Translate().CapitalizeFirst());
			Widgets.IntRange(countRect.RightPart(1 - labelWidthPercentage).Rounded(), Rand.Int, ref this.countRange, 0, 12);

			Rect levelRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			Widgets.Label(levelRect.LeftPart(labelWidthPercentage).Rounded(), "Level".Translate().CapitalizeFirst());
			Widgets.IntRange(levelRect.RightPart(1 - labelWidthPercentage).Rounded(), Rand.Int, ref this.range, 0, 20);
		}

		public override string Summary(PawnFilter filter) => this.countRange.min == this.countRange.max
			? this.range.min == this.range.max
				? "HasSkillsAtLevel".Translate(this.countRange.min, this.range.min)
				: "HasSkillsBetweenLevels".Translate(this.countRange.min, this.range.min, this.range.max)
			: this.range.min == this.range.max
				? "HasBetweenSkillsAtLevel".Translate(this.countRange.min, this.countRange.max, this.range.min)
				: "HasBetweenSkillsBetweenLevels".Translate(this.countRange.min, this.countRange.max, this.range.min, this.range.max);

		public override void Randomize() {
			this.countRange = new IntRange(Rand.Range(0, 1), Rand.Range(1, 3));
			this.range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Values.Look(ref this.countRange, nameof(this.countRange));
		}
	}
}
