#if !(V1_0 || V1_1 || V1_2)
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
	internal sealed class HasFavoriteColor : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<ColorDef> LegalFavoriteColors =>
#if V1_3
			DefDatabase<ColorDef>.AllDefs;
#else
			DefDatabase<ColorDef>.AllDefs.Where((def) => def.colorType == ColorType.Ideo || def.colorType == ColorType.Misc);
#endif

		private ColorDef _color;

		private static string ColorToHex(Color color) =>
			$"#{(int)(color.r * 0xff):X2}{(int)(color.g * 0xff):X2}{(int)(color.b * 0xff):X2}";

		private static string LabelFor(ColorDef def) =>
			(def?.LabelCap.NullOrEmpty() ?? true) ? "PM.UnnamedColorHex".Translate(ColorToHex(def.color)).CapitalizeFirst()
			: def.LabelCap;

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._color, MiscUtil.NoUnder(nameof(this._color)));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_3 || V1_4 || V1_5
			: pawn.story.favoriteColor == this._color.color;
#else
			: pawn.story.favoriteColor == this._color;
#endif

		public override string Summary(PawnFilter filter) =>
			"PM.HasFavoriteColor".Translate(LabelFor(this._color));

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, LabelFor(this._color))) {
				FloatMenuUtility.MakeMenu(LegalFavoriteColors.OrderBy((def) => def.label),
					LabelFor,
					(def) => () => this._color = def);
			}
		}

		internal override void Randomize() =>
			this._color = LegalFavoriteColors.RandomElement();
	}
}
#endif
