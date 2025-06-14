﻿#if !V1_0
using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasMeditationFocus : PawnFilterPart {
		private MeditationFocusDef meditationFocus;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: this.meditationFocus.CanPawnUse(pawn);

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.meditationFocus.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<MeditationFocusDef>.AllDefsListForReading, (def) => def.LabelCap, (def) => () => this.meditationFocus = def);
			}
		}

		public override string Summary(PawnFilter filter) => "HasMeditationFocus".Translate(this.meditationFocus.label);

		public override void Randomize() => this.meditationFocus = DefDatabase<MeditationFocusDef>.AllDefsListForReading.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.meditationFocus, nameof(this.meditationFocus));
		}
	}
}
#endif
