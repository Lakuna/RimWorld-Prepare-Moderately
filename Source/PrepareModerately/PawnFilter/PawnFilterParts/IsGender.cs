using PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately.PawnFilter.PawnFilterParts {
	public class IsGender : PawnFilterPart {
		public Gender gender;

		public IsGender() => this.gender = Gender.Male;

		public override bool Matches(Pawn pawn) => pawn.gender == this.gender;

		public override void DoEditInterface(Listing_PawnFilter listing) {
			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.gender.ToString().CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu((Gender[]) Enum.GetValues(typeof(Gender)), (gender) => gender.ToString().CapitalizeFirst(), (gender) => () => this.gender = gender);
			}
		}
	}
}
