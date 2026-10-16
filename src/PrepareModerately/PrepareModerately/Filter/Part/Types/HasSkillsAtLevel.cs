using System;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasSkillsAtLevel : PawnFilterPart {
#pragma warning restore CA1812
		private const float LabelWidthPercentage = 0.2f;

		private IntRange _countRange;

		private IntRange _range;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._range, MiscUtil.NoUnder(nameof(this._range)));
			Scribe_Values.Look(ref this._countRange, MiscUtil.NoUnder(nameof(this._countRange)));
		}

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int count = pawn.skills.skills.AsEnumerable().Count((skill) => skill.Level <= this._range.max && skill.Level >= this._range.min);
			return count <= this._countRange.max && count >= this._countRange.min;
		}

		public override string Summary(PawnFilter filter) =>
			this._countRange.min == this._countRange.max ?
				this._range.min == this._range.max ? "PM.HasSkillsAtLevel".Translate(this._countRange.min, this._range.min)
				: "PM.HasSkillsBetweenLevels".Translate(this._countRange.min, this._range.min, this._range.max)
			: this._range.min == this._range.max ? "PM.HasBetweenSkillsAtLevel".Translate(this._countRange.min, this._countRange.max, this._range.min)
			: "PM.HasBetweenSkillsBetweenLevels".Translate(this._countRange.min, this._countRange.max, this._range.min, this._range.max);

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
			Widgets.Label(countRect.LeftPart(LabelWidthPercentage).Rounded(), "PM.Count".Translate().CapitalizeFirst());
			Widgets.IntRange(countRect.RightPart(1 - LabelWidthPercentage).Rounded(), Rand.Int, ref this._countRange, 0, 12);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect levelRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
#else
			Rect levelRect = new(rect.x, countRect.yMax, rect.width, Text.LineHeight);
#endif
			Widgets.Label(levelRect.LeftPart(LabelWidthPercentage).Rounded(), "PM.Level".Translate().CapitalizeFirst());
			Widgets.IntRange(levelRect.RightPart(1 - LabelWidthPercentage).Rounded(), Rand.Int, ref this._range, 0, 20);
		}

		internal override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._countRange = new IntRange(Rand.Range(0, 1), Rand.Range(1, 3));
			this._range = new IntRange(Rand.Range(0, 5), Rand.Range(5, 10));
#else
			this._countRange = new(Rand.Range(0, 1), Rand.Range(1, 3));
			this._range = new(Rand.Range(0, 5), Rand.Range(5, 10));
#endif
		}
	}
}
