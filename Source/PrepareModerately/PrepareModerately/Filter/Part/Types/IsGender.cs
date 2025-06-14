using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsGender : PawnFilterPart {
		private Gender gender;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.gender == this.gender;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Gender[])Enum.GetValues(typeof(Gender)),
				(gender) => gender.ToString().CapitalizeFirst(),
				(gender) => () => this.gender = gender);
			}
		}

		public override string Summary(PawnFilter filter) => "IsGender".Translate(this.gender.ToString());

		public override void Randomize() => this.gender = GetRandomOfEnum(new Gender());

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref this.gender, nameof(this.gender));
		}
	}
}
