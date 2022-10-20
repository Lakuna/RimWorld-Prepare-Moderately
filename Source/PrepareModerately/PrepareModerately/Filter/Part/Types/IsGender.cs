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

		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Gender[])Enum.GetValues(typeof(Gender)),
				(Gender gender) => gender.ToString().CapitalizeFirst(),
				(Gender gender) => () => this.gender = gender);
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
