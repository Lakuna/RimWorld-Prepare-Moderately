#if !(V1_0 || V1_1 || V1_2 || V1_3)
using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasPossession : PawnFilterPart {
		private ThingDef possession;

		private static List<ThingDefCount> StartingPossessionsOf(Pawn pawn) {
			Dictionary<Pawn, List<ThingDefCount>> startingPossessions = (Dictionary<Pawn, List<ThingDefCount>>)typeof(StartingPawnUtility)
				.GetProperty("StartingPossessions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.GetValue(null, null);

			return startingPossessions.TryGetValue(pawn, out List<ThingDefCount> output) ? output : new List<ThingDefCount>();
		}

		private static readonly IEnumerable<ThingDef> PossiblePossessions = DefDatabase<ThingDef>.AllDefsListForReading.Where((def) => def.category == ThingCategory.Item);

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: StartingPossessionsOf(pawn).Any((defCount) => defCount.ThingDef == this.possession);

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			if (Widgets.ButtonText(rect, this.possession.LabelCap)) {
				FloatMenuUtility.MakeMenu(PossiblePossessions, (def) => def.LabelCap, (def) => () => this.possession = def);
			}
		}

		public override string Summary(PawnFilter filter) => "HasPossession".Translate(this.possession.label);

		public override void Randomize() => this.possession = PossiblePossessions.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.possession, nameof(this.possession));
		}
	}
}
#endif
