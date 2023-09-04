using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasPassionsAtLevel : PawnFilterPart {
		private int count;

		private Passion passion;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.skills.skills.FindAll((SkillRecord skill) => skill.passion == this.passion).Count >= this.count;

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

			Rect passionRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
			if (Widgets.ButtonText(passionRect, this.passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(
					(Passion[])Enum.GetValues(typeof(Passion)),
					(Passion passion) => passion.ToString().CapitalizeFirst(),
					(Passion passion) => () => this.passion = passion);
			}
		}

		public override string Summary(PawnFilter filter) => "HasAtLeastPassionForAtLeastSkills".Translate(this.passion.ToString(), this.count);

		public override void Randomize() {
			this.passion = GetRandomOfEnum(new Passion());
			this.count = Rand.Range(3, 6);
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.passion, nameof(this.passion));
			Scribe_Values.Look(ref this.count, nameof(this.count));
		}
	}
}
