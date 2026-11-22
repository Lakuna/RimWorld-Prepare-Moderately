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
		private static IEnumerable<Gender> _legalGenders {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Gender[] values = Enum.GetValues(typeof(Gender));
#else
				Gender[] values = Enum.GetValues<Gender>();
#endif
				return values.OfType<Gender>();
			}
		}

		private Gender _gender;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, this._gender.ToString().Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_legalGenders.OrderBy((gender) => gender.ToString()),
					(gender) => gender.ToString().Finish(CapitalizeType.First),
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
			"PM.IsGender".TranslateFull(this._gender.ToString());
	}
}
