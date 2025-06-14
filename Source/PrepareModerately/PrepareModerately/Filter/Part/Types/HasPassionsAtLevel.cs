using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasPassionsAtLevel : PawnFilterPart {
		private IntRange range;

		private Passion passion;

		public override bool Matches(Pawn pawn) {
			if (pawn == null) {
				throw new ArgumentNullException(nameof(pawn));
			}
			
			int count = pawn.skills.skills.FindAll((skill) => skill.passion == this.passion).Count;
			return count <= this.range.max && count >= this.range.min;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

			float labelWidthPercentage = 0.2f;
			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
			Widgets.Label(countRect.LeftPart(labelWidthPercentage).Rounded(), "Count".Translate().CapitalizeFirst());
			Widgets.IntRange(countRect.RightPart(1 - labelWidthPercentage), Rand.Int, ref this.range, 0, 12);

			Rect passionRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Passion[])Enum.GetValues(typeof(Passion)), (passion) => passion.ToString().CapitalizeFirst(), (passion) => () => this.passion = passion);
			}
		}

		public override string Summary(PawnFilter filter) => this.range.min == this.range.max
			? "HasPassionForSkills".Translate(this.passion.ToString(), this.range.min)
			: "HasPassionForBetweenSkills".Translate(this.passion.ToString(), this.range.min, this.range.max);

		public override void Randomize() {
			this.passion = GetRandomOfEnum(new Passion());
			this.range = new IntRange(Rand.Range(0, 2), Rand.Range(2, 6));
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Values.Look(ref this.range, nameof(this.range));
		}
	}
}
