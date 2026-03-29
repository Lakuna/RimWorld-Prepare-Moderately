#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasFavoriteColor : PawnFilterPart {
		private ColorDef color;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_3 || V1_4 || V1_5
			: pawn.story.favoriteColor == this.color.color;
#else
			: pawn.story.favoriteColor == this.color;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			if (Widgets.ButtonText(rect, this.color.LabelCap.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(this.color.color)).CapitalizeFirst() : this.color.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<ColorDef>.AllDefsListForReading.Where((def) => !def.LabelCap.NullOrEmpty()).OrderBy((def) => def.label),
					(def) => def.LabelCap.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(def.color)).CapitalizeFirst() : def.LabelCap,
					(def) => () => this.color = def);
			}
		}

		private static string ColorToHex(Color color) =>
			$"#{color.r:X2}{color.g:X2}{color.b:X2}";

		public override string Summary(PawnFilter filter) => "PM.HasFavoriteColor".Translate(this.color.LabelCap.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(this.color.color)) : this.color.LabelCap);

		public override void Randomize() => this.color = DefDatabase<ColorDef>.AllDefsListForReading.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.color, nameof(this.color));
		}
	}
}
#endif
