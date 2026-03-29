using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsSpecies : PawnFilterPart {
		private static IEnumerable<ThingDef> LegalThings => DefDatabase<ThingDef>.AllDefs.Where((def) => def.race != null);

		private ThingDef species;

		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.def == this.species;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.species.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalThings.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.species = def);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.IsSpecies".Translate(this.species.label);

		public override void Randomize() => this.species = LegalThings.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.species, nameof(this.species));
		}
	}
}
