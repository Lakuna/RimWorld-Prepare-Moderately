using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class IsKind : PawnFilterPart {
		private static IEnumerable<PawnKindDef> _legalKinds {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				List<PawnKindDef> empty = new List<PawnKindDef>();
#else
				List<PawnKindDef> empty = [];
#endif
				return DefDatabase<PawnKindDef>.AllDefs ?? empty;
			}
		}

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

		private static string GetCombinedLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnKindDef def
#else
			PawnKindDef? def
#endif
		) =>
			def is null ? "null"
			: (def.labelFemale is null || def.labelFemale.NullOrEmpty()) ? (def.label ?? "null")
			: $"{def.label ?? "null"}/{def.labelFemale}";

		private static string GetUniqueCombinedLabelFor(
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnKindDef def
#else
			PawnKindDef? def
#endif
		) =>
			def is null ? "null"
			: GetCombinedLabelFor(def).NullOrEmpty() ? (def.defName ?? "null")
			: _legalKinds.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName ?? "null"})"
			: GetCombinedLabelFor(def);

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._kind).Finish(CapitalizeType.First))) {
				FloatMenuUtility.MakeMenu(
					_legalKinds.OrderBy((def) => def.label),
					(def) => GetUniqueCombinedLabelFor(def).Finish(CapitalizeType.First),
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

		public override void Randomize() {
#if V1_0 || V1_1 || V1_2 || V1_3
			PawnKindDef startingPawnKind = null;
#elif V1_4 || V1_5 || V1_6
			PawnKindDef startingPawnKind = Find.GameInitData?.startingPawnKind;
#else
			PawnKindDef? startingPawnKind = Find.GameInitData?.startingPawnKind;
#endif
			this._kind = startingPawnKind ?? Faction.OfPlayerSilentFail?.def?.basicMemberKind ?? PawnKindDefOf.Colonist ?? _legalKinds.FirstOrDefault(); // Other kinds wouldn't be legal for vanilla RimWorld.
		}

		public override string Summary(PawnFilter filter) =>
			"PM.IsKind".TranslateFull(GetUniqueCombinedLabelFor(this._kind));
	}
}
