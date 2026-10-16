#if !V1_0
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
	internal sealed class HasMeditationFocus : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<MeditationFocusDef> LegalMeditationFoci =>
			DefDatabase<MeditationFocusDef>.AllDefs;

		private MeditationFocusDef _meditationFocus;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._meditationFocus, MiscUtil.NoUnder(nameof(this._meditationFocus)));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: this._meditationFocus.CanPawnUse(pawn);

		public override string Summary(PawnFilter filter) =>
			"PM.HasMeditationFocus".Translate(this._meditationFocus.label);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this._meditationFocus.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalMeditationFoci.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this._meditationFocus = def);
			}
		}

		internal override void Randomize() =>
			this._meditationFocus = LegalMeditationFoci.RandomElement();
	}
}
#endif
