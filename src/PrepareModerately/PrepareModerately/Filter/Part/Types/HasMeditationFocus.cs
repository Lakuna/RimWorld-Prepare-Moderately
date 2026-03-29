#if !V1_0
using System;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasMeditationFocus : PawnFilterPart {
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
				FloatMenuUtility.MakeMenu(DefDatabase<MeditationFocusDef>.AllDefsListForReading.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.meditationFocus = def);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.HasMeditationFocus".Translate(this.meditationFocus.label);

		public override void Randomize() => this.meditationFocus = DefDatabase<MeditationFocusDef>.AllDefsListForReading.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.meditationFocus, nameof(this.meditationFocus));
		}
	}
}
#endif
