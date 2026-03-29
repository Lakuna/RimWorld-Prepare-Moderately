#if !V1_0
using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasMeditationFocus : PawnFilterPart {
		private static IEnumerable<MeditationFocusDef> LegalMeditationFoci => DefDatabase<MeditationFocusDef>.AllDefs;

		private MeditationFocusDef meditationFocus;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: this.meditationFocus.CanPawnUse(pawn);

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.meditationFocus.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalMeditationFoci.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.meditationFocus = def);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.HasMeditationFocus".Translate(this.meditationFocus.label);

		public override void Randomize() => this.meditationFocus = LegalMeditationFoci.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.meditationFocus, nameof(this.meditationFocus));
		}
	}
}
#endif
