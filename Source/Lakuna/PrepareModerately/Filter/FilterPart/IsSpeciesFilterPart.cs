using System.Collections.Generic;
using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsSpeciesFilterPart : FilterPart {
		// TODO: Currently, this filter allows the player to choose between any humanlike race. This might not be equal to the list of startable races. Make it be.
		private static readonly IEnumerable<ThingDef> humanlikeDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef def) => def.race != null && def.race.Humanlike);

		public ThingDef species;

		public override bool Matches(Pawn pawn) {
			return pawn.def == this.species;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.species.LabelCap)) {
				FloatMenuUtility.MakeMenu(humanlikeDefs,
					(ThingDef def) => def.LabelCap,
					(ThingDef def) => () => this.species = def);
			}
		}

		public override string Summary(Filter filter) {
			return "IsSpecies".Translate(this.species.label);
		}

		public override void Randomize() {
			this.species = IsSpeciesFilterPart.humanlikeDefs.ToArray()[Rand.Range(0, IsSpeciesFilterPart.humanlikeDefs.Count() - 1)];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.species, nameof(this.species));
		}
	}
}
