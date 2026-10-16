#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasPossession : PawnFilterPart {
#pragma warning restore CA1812
		// See `Verse.StartingPawnUtility.GeneratePossessions`.
		private static IEnumerable<ThingDef> LegalThings =>
#if V1_4
			new ThingDef[] { ThingDefOf.BabyFood }
				.Concat(DefDatabase<ThingDef>.AllDefs.Where((def) => def.GetCompProperties<CompProperties_Drug>()?.chemical != null))
				.Concat(ThingDefOf.HemogenPack)
				.Concat(DefDatabase<BackstoryDef>.AllDefs.SelectMany((def) => def.possessions.Select((def2) => def2.key)))
				.Concat(DefDatabase<ThingDef>.AllDefs.Where((def) => def.possessionCount > 0))
				.Distinct();
#elif V1_5 || V1_6
			new ThingDef[] { ThingDefOf.BabyFood }
				.Concat(DefDatabase<ThingDef>.AllDefs.Where((def) => def.GetCompProperties<CompProperties_Drug>()?.chemical != null))
				.Concat(ThingDefOf.HemogenPack)
				.Concat(DefDatabase<BackstoryDef>.AllDefs.SelectMany((def) => def.possessions.Select((def2) => def2.key)))
				.Concat(TraitDegreePair.TraitDegreePairs.SelectMany((pair) => pair.TraitDegreeData.possessions.Select((def) => def.key)))
				.Concat(DefDatabase<ThingDef>.AllDefs.Where((def) => def.possessionCount > 0))
				.Distinct();
#else
			new[] { ThingDefOf.BabyFood }
				.Concat(DefDatabase<ThingDef>.AllDefs.Where((def) => def.GetCompProperties<CompProperties_Drug>()?.chemical != null))
				.Concat(ThingDefOf.HemogenPack)
				.Concat(DefDatabase<BackstoryDef>.AllDefs.SelectMany((def) => def.possessions.Select((def2) => def2.key)))
				.Concat(TraitDegreePair.TraitDegreePairs.SelectMany((pair) => pair.TraitDegreeData.possessions.Select((def) => def.key)))
				.Concat(DefDatabase<ThingDef>.AllDefs.Where((def) => def.possessionCount > 0))
				.Distinct();
#endif

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private ThingDef possession;
#pragma warning restore IDE1006

		private static Collection<ThingDefCount> StartingPossessionsOf(Pawn pawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Collection<ThingDefCount>((Find.GameInitData?.startingPossessions ?? new Dictionary<Pawn, List<ThingDefCount>>()).TryGetValue(pawn, out List<ThingDefCount> output) ? output : new List<ThingDefCount>());
#else
			new((Find.GameInitData?.startingPossessions ?? []).TryGetValue(pawn, out List<ThingDefCount> output) ? output : []);
#endif

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.possession, nameof(this.possession));
		}

		internal override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
			: StartingPossessionsOf(pawn).Any((defCount) => defCount.ThingDef == this.possession);

		internal override string Summary(PawnFilter filter) =>
			"PM.HasPossession".Translate(this.possession.label);

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);

			if (Widgets.ButtonText(rect, this.possession.LabelCap)) {
				FloatMenuUtility.MakeMenu(LegalThings.OrderBy((def) => def.label),
					(def) => def.LabelCap,
					(def) => () => this.possession = def);
			}
		}

		internal override void Randomize() =>
			this.possession = LegalThings.RandomElement();
	}
}
#endif
