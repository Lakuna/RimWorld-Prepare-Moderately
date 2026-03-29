using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsGender : PawnFilterPart {
		private static IEnumerable<Gender> LegalGenders => Enum.GetValues(typeof(Gender)).OfType<Gender>();

		private Gender gender;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.gender == this.gender;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalGenders.OrderBy((gender) => gender.ToString()),
				(gender) => gender.ToString().CapitalizeFirst(),
				(gender) => () => this.gender = gender);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.IsGender".Translate(this.gender.ToString());

		public override void Randomize() => this.gender = LegalGenders.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.gender, nameof(this.gender));
		}
	}
}
