#if !(V1_0 || V1_1 || V1_2)
using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasFavoriteColor : PawnFilterPart {
		private ColorDef color;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.Ideo.colorDef == this.color;

		public override void DoEditInterface(PawnFilterEditListing listing) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			Rect rect = listing.GetPawnFilterPartRect(this, Text.LineHeight);

			if (Widgets.ButtonText(rect, this.color.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<ColorDef>.AllDefsListForReading,
					(ColorDef def) => def.LabelCap,
					(ColorDef def) => () => this.color = def);
			}
		}

		public override string Summary(PawnFilter filter) => "HasFavoriteColor".Translate(this.color.label);

		public override void Randomize() => this.color = GetRandomOfDef<ColorDef>();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.color, nameof(this.color));
		}
	}
}
#endif
