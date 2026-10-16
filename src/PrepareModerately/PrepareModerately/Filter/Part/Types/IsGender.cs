using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class IsGender : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<Gender> _legalGenders =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(Gender)).OfType<Gender>();
#else
			Enum.GetValues<Gender>().OfType<Gender>();
#endif

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private Gender gender;
#pragma warning restore IDE1006

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.gender, nameof(this.gender));
		}

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(_legalGenders.OrderBy((gender) => gender.ToString()),
				(gender) => gender.ToString().CapitalizeFirst(),
				(gender) => () => this.gender = gender);
			}
		}

		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: pawn.gender == this.gender;

		internal override void Randomize() =>
			this.gender = _legalGenders.RandomElement();

		internal override string Summary(PawnFilter filter) =>
			"PM.IsGender".Translate(this.gender.ToString());
	}
}
