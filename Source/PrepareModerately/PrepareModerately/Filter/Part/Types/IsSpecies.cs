using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class IsSpecies : PawnFilterPart {
		private static IEnumerable<ThingDef> AllHumanlikeThingDefs {
			get {
				foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading) {
					if (def.race != null && def.race.Humanlike) { yield return def; }
				}
			}
		}

		private ThingDef species;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.def == this.species;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.species.LabelCap)) {
				FloatMenuUtility.MakeMenu(AllHumanlikeThingDefs,
					(ThingDef def) => def.LabelCap,
					(ThingDef def) => () => this.species = def);
			}
		}

		public override string Summary(PawnFilter filter) => "IsSpecies".Translate(this.species.label);

		public override void Randomize() => this.species = AllHumanlikeThingDefs.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.species, nameof(this.species));
		}
	}
}
