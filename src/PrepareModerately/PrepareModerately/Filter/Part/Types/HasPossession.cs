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
				List<ThingDef> empty0 = new List<ThingDef>();
#else
				List<ThingDef> empty0 = [];
#endif
				foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs ?? empty0) {
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
				List<BackstoryDef> empty1 = new List<BackstoryDef>();
#else
				List<BackstoryDef> empty1 = [];
#endif
				foreach (BackstoryDef def in DefDatabase<BackstoryDef>.AllDefs ?? empty1) {
#if V1_4
					List<BackstoryThingDefCountClass> possessions = def.possessions ?? new List<BackstoryThingDefCountClass>();
#elif V1_5 || V1_6
					List<PossessionThingDefCountClass> possessions = def.possessions ?? new List<PossessionThingDefCountClass>();
#else
					List<PossessionThingDefCountClass> possessions = def.possessions ?? [];
#endif
					foreach (
#if V1_4
						BackstoryThingDefCountClass possession in possessions
#else
						PossessionThingDefCountClass possession in possessions
#endif
					) {
						if (possession.key is null) {
							continue;
						}

						o.Add(possession.key);
					}
				}

#if !V1_4
#if V1_5 || V1_6
				List<PossessionThingDefCountClass> empty2 = new List<PossessionThingDefCountClass>();
#else
				List<PossessionThingDefCountClass> empty2 = [];
#endif
				// Certain traits add special possible possessions.
				foreach (TraitDegree traitDegree in TraitDegree.All) {
					foreach (PossessionThingDefCountClass possession in traitDegree.Data.possessions ?? empty2) {
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

		private static Collection<ThingDefCount> StartingPossessionsOf(Pawn pawn) {
#if V1_4 || V1_5 || V1_6
			Dictionary<Pawn, List<ThingDefCount>> empty0 = new Dictionary<Pawn, List<ThingDefCount>>();
#else
			Dictionary<Pawn, List<ThingDefCount>> empty0 = [];
#endif
			Dictionary<Pawn, List<ThingDefCount>> startingPossessions = Find.GameInitData?.startingPossessions ?? empty0;
#if V1_4 || V1_5 || V1_6
			List<ThingDefCount> empty1 = new List<ThingDefCount>();
#else
			List<ThingDefCount> empty1 = [];
#endif
#if V1_4 || V1_5 || V1_6
			List<ThingDefCount> possessions = startingPossessions.TryGetValue(pawn, out List<ThingDefCount> output) ? output : empty1;
#else
			List<ThingDefCount> possessions = startingPossessions.TryGetValue(pawn, out List<ThingDefCount>? output) ? output : empty1;
#endif
#if V1_4 || V1_5 || V1_6
			return new Collection<ThingDefCount>(possessions);
#else
			return [.. possessions];
#endif
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, this._possession?.LabelCap.Finish() ?? "null")) {
				FloatMenuUtility.MakeMenu(
					_legalThings.OrderBy((def) => def.label),
					(def) => def.LabelCap.Finish(),
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
			"PM.HasPossession".TranslateFull(this._possession?.label ?? "null");
	}
}
#endif
