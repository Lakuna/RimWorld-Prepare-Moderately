#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasFavoriteColor : PawnFilterPart {
		private static IEnumerable<ColorDef> LegalFavoriteColors => DefDatabase<ColorDef>.AllDefs
#if !V1_3
			.Where((def) => def.colorType == ColorType.Ideo || def.colorType == ColorType.Misc)
#endif
			;

		private ColorDef color;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
#if V1_3 || V1_4 || V1_5
			: pawn.story.favoriteColor == this.color.color;
#else
			: pawn.story.favoriteColor == this.color;
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			if (Widgets.ButtonText(rect, this.color.LabelCap.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(this.color.color)).CapitalizeFirst() : this.color.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalFavoriteColors.OrderBy((def) => def.label),
					(def) => def.LabelCap.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(def.color)).CapitalizeFirst() : def.LabelCap,
					(def) => () => this.color = def);
			}
		}

		private static string ColorToHex(Color color) =>
			$"#{(int)color.r:X2}{(int)color.g:X2}{(int)color.b:X2}";

		public override string Summary(PawnFilter filter) => "PM.HasFavoriteColor".Translate(this.color.LabelCap.NullOrEmpty() ? "PM.UnnamedColorHex".Translate(ColorToHex(this.color.color)) : this.color.LabelCap);

		public override void Randomize() => this.color = LegalFavoriteColors.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.color, nameof(this.color));
		}
	}
}
#endif
