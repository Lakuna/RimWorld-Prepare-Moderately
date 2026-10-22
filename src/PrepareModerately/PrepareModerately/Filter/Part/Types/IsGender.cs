using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsGender : PawnFilterPart {
		private static IEnumerable<Gender> _legalGenders =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Gender)).OfType<Gender>();
#else
			Enum.GetValues<Gender>().OfType<Gender>();
#endif

		private Gender _gender;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, this._gender.ToString().CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(rect, this._gender.ToString().CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalGenders.OrderBy((gender) => gender.ToString()),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(gender) => gender.ToString().CapitalizeFirstSafe(),
#else
					(gender) => gender.ToString().CapitalizeFirstSafe,
#endif
					(gender) => () => this._gender = gender);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._gender, "gender");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ this._gender.GetHashCode();

		public override bool Matches(Pawn pawn) =>
			pawn.gender == this._gender;

		public override void Randomize() =>
			this._gender = _legalGenders.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.IsGender".Translate(this._gender.ToString()) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.IsGender".Translate(this._gender.ToString()).StringSafe();
#else
			"PM.IsGender".Translate(this._gender.ToString()).StringSafe;
#endif
	}
}
