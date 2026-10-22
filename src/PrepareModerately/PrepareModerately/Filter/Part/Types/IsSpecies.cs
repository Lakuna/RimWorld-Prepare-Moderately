using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsSpecies : PawnFilterPart {
		private static IEnumerable<ThingDef> _legalThings =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<ThingDef>.AllDefs?.Where((def) => def?.race != null && !def.IsCorpse) ?? new List<ThingDef>();
#else
			DefDatabase<ThingDef>.AllDefs?.Where((def) => def?.race is not null && !def.IsCorpse) ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._species is null) {
					yield return "Is species filter part species is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._species is null;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private ThingDef _species;
#else
		private ThingDef? _species;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetUniqueLabelFor(ThingDef def) =>
#else
		private static string GetUniqueLabelFor(ThingDef? def) =>
#endif
			def is null ? "null"
			: def.label.NullOrEmpty() ? (def.defName ?? "null")
			: _legalThings.Count((def2) => def.label == def2.label) > 1 ? $"{def.label} ({def.defName ?? "null"})"
			: def.label;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this._species).CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this._species).CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalThings.OrderBy((def) => def.label),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => GetUniqueLabelFor(def).CapitalizeFirstSafe(),
#else
					(def) => GetUniqueLabelFor(def).CapitalizeFirstSafe,
#endif
					(def) => () => this._species = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._species, "species");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._species?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
			pawn.def == this._species;

		public override void Randomize() =>
			this._species = ThingDefOf.Human ?? _legalThings.First(); // Other things wouldn't be legal for vanilla RimWorld.

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.IsSpecies".Translate(GetUniqueLabelFor(this._species)) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.IsSpecies".Translate(GetUniqueLabelFor(this._species)).StringSafe();
#else
			"PM.IsSpecies".Translate(GetUniqueLabelFor(this._species)).StringSafe;
#endif
	}
}
