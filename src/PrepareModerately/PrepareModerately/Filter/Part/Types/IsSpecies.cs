using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsSpecies : PawnFilterPart {
		private static IEnumerable<ThingDef> _legalThings {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				List<ThingDef> empty = new List<ThingDef>();
#else
				List<ThingDef> empty = [];
#endif
				return DefDatabase<ThingDef>.AllDefs?.Where((def) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					!(def?.race is null)
#else
					def?.race is not null
#endif
					&& !def.IsCorpse
				) ?? empty;
			}
		}

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

		private static string GetUniqueLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			ThingDef def
#else
			ThingDef? def
#endif
		) =>
			def is null ? "null"
			: (def.label is null || def.label.NullOrEmpty()) ? (def.defName ?? "null")
			: _legalThings.Count((def2) => def.label == def2.label) > 1 ? $"{def.label} ({def.defName ?? "null"})"
			: def.label;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, GetUniqueLabelFor(this._species).Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_legalThings.OrderBy((def) => def.label),
					(def) => GetUniqueLabelFor(def).Finish(CapitalizeType.First),
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
			this._species = ThingDefOf.Human ?? _legalThings.FirstOrDefault(); // Other things wouldn't be legal for vanilla RimWorld.

		public override string Summary(PawnFilter filter) =>
			"PM.IsSpecies".TranslateFull(GetUniqueLabelFor(this._species));
	}
}
