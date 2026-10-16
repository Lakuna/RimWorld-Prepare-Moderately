using System;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasSkillsAtLevel : PawnFilterPart {
#pragma warning restore CA1812
		private const float _labelWidthPercentage = 0.2f;

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private IntRange countRange;

		private IntRange range;
#pragma warning restore IDE1006

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.range, nameof(this.range));
			Scribe_Values.Look(ref this.countRange, nameof(this.countRange));
		}

		internal override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int count = pawn.skills.skills.AsEnumerable().Count((skill) => skill.Level <= this.range.max && skill.Level >= this.range.min);
			return count <= this.countRange.max && count >= this.countRange.min;
		}

		internal override string Summary(PawnFilter filter) =>
			this.countRange.min == this.countRange.max ?
				this.range.min == this.range.max ? "PM.HasSkillsAtLevel".Translate(this.countRange.min, this.range.min)
				: "PM.HasSkillsBetweenLevels".Translate(this.countRange.min, this.range.min, this.range.max)
			: this.range.min == this.range.max ? "PM.HasBetweenSkillsAtLevel".Translate(this.countRange.min, this.countRange.max, this.range.min)
			: "PM.HasBetweenSkillsBetweenLevels".Translate(this.countRange.min, this.countRange.max, this.range.min, this.range.max);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
#else
			Rect countRect = new(rect.x, rect.y, rect.width, Text.LineHeight);
#endif
			Widgets.Label(countRect.LeftPart(_labelWidthPercentage).Rounded(), "PM.Count".Translate().CapitalizeFirst());
			Widgets.IntRange(countRect.RightPart(1 - _labelWidthPercentage).Rounded(), Rand.Int, ref this.countRange, 0, 12);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect levelRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
#else
			Rect levelRect = new(rect.x, countRect.yMax, rect.width, Text.LineHeight);
#endif
			Widgets.Label(levelRect.LeftPart(_labelWidthPercentage).Rounded(), "PM.Level".Translate().CapitalizeFirst());
			Widgets.IntRange(levelRect.RightPart(1 - _labelWidthPercentage).Rounded(), Rand.Int, ref this.range, 0, 20);
		}

		internal override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this.countRange = new IntRange(Rand.Range(0, 1), Rand.Range(1, 3));
			this.range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
#else
			this.countRange = new(Rand.Range(0, 1), Rand.Range(1, 3));
			this.range = new(Rand.Range(0, 5), Rand.Range(5, 10));
#endif
		}
	}
}
