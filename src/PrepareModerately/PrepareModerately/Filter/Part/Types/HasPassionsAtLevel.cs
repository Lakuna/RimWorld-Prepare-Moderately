using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasPassionsAtLevel : PawnFilterPart {
#pragma warning restore CA1812
		private const float LabelWidthPercentage = 0.2f;

		private static IEnumerable<Passion> LegalPassions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Passion)).OfType<Passion>();
#else
			Enum.GetValues<Passion>().OfType<Passion>();
#endif

		private Passion _passion;

		private IntRange _range;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._passion, MiscUtil.NoUnder(nameof(this._passion)));
			Scribe_Values.Look(ref this._range, MiscUtil.NoUnder(nameof(this._range)));
		}

		public override bool Matches(Pawn pawn) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}
#else
			ArgumentNullException.ThrowIfNull(pawn);
#endif

			int count = pawn.skills.skills.AsEnumerable().Count((skill) => skill.passion == this._passion);
			return count <= this._range.max && count >= this._range.min;
		}

		public override string Summary(PawnFilter filter) =>
			this._range.min == this._range.max ? "PM.HasPassionForSkills".Translate(this._passion.ToString(), this._range.min)
			: "PM.HasPassionForBetweenSkills".Translate(this._passion.ToString(), this._range.min, this._range.max);

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
			Widgets.IntRange(countRect.RightPart(1 - LabelWidthPercentage), Rand.Int, ref this._range, 0, 12);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Rect passionRect = new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight);
#else
			Rect passionRect = new(rect.x, countRect.yMax, rect.width, Text.LineHeight);
#endif
			if (Widgets.ButtonText(passionRect, this._passion.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalPassions.OrderBy((passion) => passion.ToString()),
					(passion) => passion.ToString().CapitalizeFirst(),
					(passion) => () => this._passion = passion);
			}
		}

		internal override void Randomize() {
			this._passion = LegalPassions.RandomElement();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(0, 2), Rand.Range(2, 6));
#else
			this._range = new(Rand.Range(0, 2), Rand.Range(2, 6));
#endif
		}
	}
}
