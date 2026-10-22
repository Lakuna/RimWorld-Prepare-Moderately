#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasPossession : PawnFilterPart {
		// See `Verse.StartingPawnUtility.GeneratePossessions`.
		private static IEnumerable<ThingDef> _legalThings {
			get {
#if V1_4 || V1_5 || V1_6
				Collection<ThingDef> o = new Collection<ThingDef>();
#else
				Collection<ThingDef> o = [];
#endif

				// Babies can start with baby food.
				o.Add(ThingDefOf.BabyFood);

#if V1_4 || V1_5 || V1_6
				foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs ?? new List<ThingDef>()) {
#else
				foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs ?? []) {
#endif
					// Pawns with addictions or chemical dependencies can start with some of whatever they're addicted to or dependent on.
					if (def.GetCompProperties<CompProperties_Drug>()?.chemical is null) {
						o.Add(def);
						continue;
					}

					// Certain items can randomly be starting possessions.
					if (def.possessionCount > 0) {
						o.Add(def);
						continue;
					}
				}

				// Hemogenic pawns can start with hemogen packs.
				o.Add(ThingDefOf.HemogenPack);

				// Certain backstories add special possible possessions.
#if V1_4 || V1_5 || V1_6
				foreach (BackstoryDef def in DefDatabase<BackstoryDef>.AllDefs ?? new List<BackstoryDef>()) {
#else
				foreach (BackstoryDef def in DefDatabase<BackstoryDef>.AllDefs ?? []) {
#endif
#if V1_4
					foreach (BackstoryThingDefCountClass possession in def.possessions ?? new List<BackstoryThingDefCountClass>()) {
#elif V1_5 || V1_6
					foreach (PossessionThingDefCountClass possession in def.possessions ?? new List<PossessionThingDefCountClass>()) {
#else
					foreach (PossessionThingDefCountClass possession in def.possessions ?? []) {
#endif
						if (possession.key is null) {
							continue;
						}

						o.Add(possession.key);
					}
				}

#if V1_4
#elif V1_5 || V1_6
				// Certain traits add special possible possessions.
				foreach (TraitDegree traitDegree in TraitDegree.All) {
					foreach (PossessionThingDefCountClass possession in traitDegree.Data.possessions ?? new List<PossessionThingDefCountClass>()) {
						if (possession.key is null) {
							continue;
						}

						o.Add(possession.key);
					}
				}
#else
				// Certain traits add special possible possessions.
				foreach (TraitDegree traitDegree in TraitDegree.All) {
					foreach (PossessionThingDefCountClass possession in traitDegree.Data.possessions ?? []) {
						if (possession.key is null) {
							continue;
						}

						o.Add(possession.key);
					}
				}
#endif

				return o.Distinct();
			}
		}

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._possession is null) {
					yield return "Has possession filter part possession is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._possession is null;

#if V1_4 || V1_5 || V1_6
		private ThingDef _possession;
#else
		private ThingDef? _possession;
#endif

		private static Collection<ThingDefCount> StartingPossessionsOf(Pawn pawn) =>
#if V1_4 || V1_5 || V1_6
			new Collection<ThingDefCount>((Find.GameInitData?.startingPossessions ?? new Dictionary<Pawn, List<ThingDefCount>>()).TryGetValue(pawn, out List<ThingDefCount> output) ? output : new List<ThingDefCount>());
#else
			[.. (Find.GameInitData?.startingPossessions ?? []).TryGetValue(pawn, out List<ThingDefCount>? output) ? output : []];
#endif

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, this._possession?.LabelCap.StringSafe() ?? "null")) {
#else
			if (Widgets.ButtonText(rect, this._possession?.LabelCap.StringSafe ?? "null")) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalThings.OrderBy((def) => def.label),
#if V1_4 || V1_5 || V1_6
					(def) => def.LabelCap.StringSafe(),
#else
					(def) => def.LabelCap.StringSafe,
#endif
					(def) => () => this._possession = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._possession, "possession");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._possession?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
			StartingPossessionsOf(pawn).Any((defCount) => defCount.ThingDef == this._possession);

		public override void Randomize() =>
			this._possession = _legalThings.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_4 || V1_5 || V1_6
			"PM.HasPossession".Translate(this._possession?.label ?? "null").StringSafe();
#else
			"PM.HasPossession".Translate(this._possession?.label ?? "null").StringSafe;
#endif
	}
}
#endif
