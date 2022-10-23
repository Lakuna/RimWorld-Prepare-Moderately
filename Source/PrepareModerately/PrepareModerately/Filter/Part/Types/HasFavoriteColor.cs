using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !V1_0
using System.Threading.Tasks;
#endif
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasFavoriteColor : PawnFilterPart {
#if !(V1_0 || V1_1 || V1_2)
		private ColorDef color;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.story.favoriteColor == this.color.color;

		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);

			if (Widgets.ButtonText(rect, this.color.LabelCap.NullOrEmpty() ? "UnnamedColor".Translate().CapitalizeFirst() : this.color.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<ColorDef>.AllDefsListForReading,
					(ColorDef def) => def.LabelCap.NullOrEmpty() ? "UnnamedColor".Translate().CapitalizeFirst() : def.LabelCap,
					(ColorDef def) => () => this.color = def);
			}
		}

		public override string Summary(PawnFilter filter) => "HasFavoriteColor".Translate(this.color.LabelCap.NullOrEmpty() ? "UnnamedColor".Translate() : this.color.LabelCap);

		public override void Randomize() => this.color = DefDatabase<ColorDef>.AllDefsListForReading.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.color, nameof(this.color));
		}
#endif
	}
}
