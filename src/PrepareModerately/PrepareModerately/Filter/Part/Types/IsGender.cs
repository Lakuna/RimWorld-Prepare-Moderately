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
	internal sealed class IsGender : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<Gender> LegalGenders =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Gender)).OfType<Gender>();
#else
			Enum.GetValues<Gender>().OfType<Gender>();
#endif

		private Gender _gender;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this._gender, MiscUtil.NoUnder(nameof(this._gender)));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.gender == this._gender;

		public override string Summary(PawnFilter filter) =>
			"PM.IsGender".Translate(this._gender.ToString());

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this._gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalGenders.OrderBy((gender) => gender.ToString()),
				(gender) => gender.ToString().CapitalizeFirst(),
				(gender) => () => this._gender = gender);
			}
		}

		internal override void Randomize() =>
			this._gender = LegalGenders.RandomElement();
	}
}
