using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasPassionsAtLevel : PawnFilterPart {
		private static IEnumerable<Passion> _legalPassions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Passion)).OfType<Passion>();
#else
			Enum.GetValues<Passion>().OfType<Passion>();
#endif

		private Passion _passion;

		private IntRange _range;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight * 2, out totalAddedListHeight).Rounded();

			Rect countRect = new Rect(rect.x, rect.y, rect.width, Text.LineHeight).Rounded();
			Widgets.IntRange(countRect, Rand.Int, ref this._range, 0, 12, "PM.RangePassions");

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight).Rounded(), this._passion.ToString().CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(new Rect(rect.x, countRect.yMax, rect.width, Text.LineHeight).Rounded(), this._passion.ToString().CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalPassions.OrderBy((passion) => passion.ToString()),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(passion) => passion.ToString().CapitalizeFirstSafe(),
#else
					(passion) => passion.ToString().CapitalizeFirstSafe,
#endif
					(passion) => () => this._passion = passion);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._passion, "passion");
			Scribe_Values.Look(ref this._range, "range");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._passion.GetHashCode() ^ this._range.GetHashCode();

		public override bool Matches(Pawn pawn) {
			int count = pawn.skills?.skills?.AsEnumerable().Count((skill) => skill?.passion == this._passion) ?? 0;
			return count <= this._range.max && count >= this._range.min;
		}

		public override void Randomize() {
			this._passion = _legalPassions.RandomElement();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._range = new IntRange(Rand.Range(0, 2), Rand.Range(2, 6));
#else
			this._range = new(Rand.Range(0, 2), Rand.Range(2, 6));
#endif
		}

		public override string Summary(PawnFilter filter) =>
			(this._range.min == this._range.max ? "PM.HasPassionForSkills".Translate(this._passion.ToString(), this._range.min)
				: "PM.HasPassionForBetweenSkills".Translate(this._passion.ToString(), this._range.min, this._range.max))
#if V1_0
				?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				.StringSafe();
#else
				.StringSafe;
#endif
	}
}
