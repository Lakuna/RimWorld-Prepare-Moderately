using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsKind : PawnFilterPart {
		private static IEnumerable<PawnKindDef> _legalKinds =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<PawnKindDef>.AllDefs ?? new List<PawnKindDef>();
#else
			DefDatabase<PawnKindDef>.AllDefs ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._kind is null) {
					yield return "Is kind filter part kind is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._kind is null;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private PawnKindDef _kind;
#else
		private PawnKindDef? _kind;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetCombinedLabelFor(PawnKindDef def) =>
#else
		private static string GetCombinedLabelFor(PawnKindDef? def) =>
#endif
			def is null ? "null"
			: (def.labelFemale is null || def.labelFemale.NullOrEmpty()) ? (def.label ?? "null")
			: $"{def.label ?? "null"}/{def.labelFemale}";

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetUniqueCombinedLabelFor(PawnKindDef def) =>
#else
		private static string GetUniqueCombinedLabelFor(PawnKindDef? def) =>
#endif
			def is null ? "null"
			: GetCombinedLabelFor(def).NullOrEmpty() ? (def.defName ?? "null")
			: _legalKinds.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName ?? "null"})"
			: GetCombinedLabelFor(def);

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._kind).CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._kind).CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalKinds.OrderBy((def) => def.label),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirstSafe(),
#else
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirstSafe,
#endif
					(def) => () => this._kind = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._kind, "kind");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._kind?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
			pawn.kindDef == this._kind;

		public override void Randomize() =>
#if V1_0 || V1_1 || V1_2 || V1_3
			this._kind = Faction.OfPlayerSilentFail?.def?.basicMemberKind ?? PawnKindDefOf.Colonist ?? _legalKinds.First(); // Other kinds wouldn't be legal for vanilla RimWorld.
#else
			this._kind = Find.GameInitData?.startingPawnKind ?? Faction.OfPlayerSilentFail?.def?.basicMemberKind ?? PawnKindDefOf.Colonist ?? _legalKinds.First(); // Other kinds wouldn't be legal for vanilla RimWorld.
#endif

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.IsKind".Translate(GetUniqueCombinedLabelFor(this._kind)) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.IsKind".Translate(GetUniqueCombinedLabelFor(this._kind)).StringSafe();
#else
			"PM.IsKind".Translate(GetUniqueCombinedLabelFor(this._kind)).StringSafe;
#endif
	}
}
