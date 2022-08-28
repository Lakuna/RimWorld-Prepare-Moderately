using System.Collections.Generic;
using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsSpeciesFilterPart : FilterPart {
		// TODO: Currently, this filter allows the player to choose between any humanlike race. This might not be equal to the list of startable races. Make it be.
		private static List<ThingDef> AllHumanlikeThingDefs {
			get {
				List<ThingDef> output = new List<ThingDef>();
				foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading) {
					if (def.race != null && def.race.Humanlike) { output.Add(def); }
				}
				return output;
			}
		}

		public ThingDef species;

		public override bool Matches(Pawn pawn) {
			return pawn.def == this.species;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.species.LabelCap)) {
				FloatMenuUtility.MakeMenu(IsSpeciesFilterPart.AllHumanlikeThingDefs,
					(ThingDef def) => def.LabelCap,
					(ThingDef def) => () => this.species = def);
			}
		}

		public override string Summary(Filter filter) {
			return "IsSpecies".Translate(this.species.label);
		}

		public override void Randomize() {
			this.species = IsSpeciesFilterPart.AllHumanlikeThingDefs[Rand.Range(0, IsSpeciesFilterPart.AllHumanlikeThingDefs.Count() - 1)];
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.species, nameof(this.species));
		}
	}
}
