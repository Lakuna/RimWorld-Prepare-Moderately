#if !(V1_0 || V1_1 || V1_2)
using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
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

			if (Widgets.ButtonText(rect, this.color.LabelCap.NullOrEmpty() ? "UnnamedColor".Translate().CapitalizeFirst() : this.color.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<ColorDef>.AllDefsListForReading,
					(def) => def.LabelCap.NullOrEmpty() ? "UnnamedColor".Translate().CapitalizeFirst() : def.LabelCap,
					(def) => () => this.color = def);
			}
		}

		public override string Summary(PawnFilter filter) => "HasFavoriteColor".Translate(this.color.LabelCap.NullOrEmpty() ? "UnnamedColor".Translate() : this.color.LabelCap);

		public override void Randomize() => this.color = DefDatabase<ColorDef>.AllDefsListForReading.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.color, nameof(this.color));
		}
	}
}
#endif
