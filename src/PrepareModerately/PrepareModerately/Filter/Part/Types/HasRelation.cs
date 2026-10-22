using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasRelation : PawnFilterPart {
		private static IEnumerable<PawnRelationDef> _legalRelations =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			DefDatabase<PawnRelationDef>.AllDefs ?? new List<PawnRelationDef>();
#else
			DefDatabase<PawnRelationDef>.AllDefs ?? [];
#endif

		public override IEnumerable<string> ConfigErrors {
			get {
				foreach (string item in base.ConfigErrors) {
					yield return item;
				}

				if (this._relation is null) {
					yield return "Has relation filter part relation is null.";
				}
			}
		}

		public override bool HasNullDefs =>
			base.HasNullDefs || this._relation is null;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private PawnRelationDef _relation;
#else
		private PawnRelationDef? _relation;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetCombinedLabelFor(PawnRelationDef def) =>
#else
		private static string GetCombinedLabelFor(PawnRelationDef? def) =>
#endif
			def is null ? "null"
			: (def.labelFemale is null || def.labelFemale.NullOrEmpty()) ? (def.label ?? "null")
			: $"{def.label ?? "null"}/{def.labelFemale}";

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static string GetUniqueCombinedLabelFor(PawnRelationDef def) =>
#else
		private static string GetUniqueCombinedLabelFor(PawnRelationDef? def) =>
#endif
			def is null ? "null"
			: GetCombinedLabelFor(def).NullOrEmpty() ? (def.defName ?? "null")
			: _legalRelations.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName ?? "null"})"
			: GetCombinedLabelFor(def);

		public override bool CanCoexistWith(PawnFilterPart other) =>
			other.Def != PawnFilterPartDefOf.HasAnyRelation;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._relation).CapitalizeFirstSafe())) {
#else
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this._relation).CapitalizeFirstSafe)) {
#endif
				FloatMenuUtility.MakeMenu(
					_legalRelations.OrderBy(GetUniqueCombinedLabelFor),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirstSafe(),
#else
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirstSafe,
#endif
					(def) => () => this._relation = def);
			}
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this._relation, "relation");
		}

		public override int GetHashCode() =>
			base.GetHashCode() ^ (this._relation?.GetHashCode() ?? 0);

		public override bool Matches(Pawn pawn) =>
#if !(V1_0 || V1_1 || V1_2)
			!pawn.relations.hidePawnRelations &&
#endif
			pawn.relations.RelatedPawns.Any((relatedPawn) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					relatedPawn != null
#else
					relatedPawn is not null
#endif
					&& !((relatedPawn.RaceProps?.Animal ?? false) && relatedPawn.Dead && relatedPawn.Corpse is null)
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					&& relatedPawn.Name != null
#else
					&& relatedPawn.Name is not null
#endif
					&& !relatedPawn.Name.Numerical
#if !(V1_0 || V1_1 || V1_2)
					&& !(relatedPawn.relations?.hidePawnRelations ?? false)
#endif
					&& (relatedPawn.relations?.everSeenByPlayer ?? false)
					&& (pawn.GetRelations(relatedPawn)?.Any((relation) => relation == this._relation) ?? false));

		public override void Randomize() =>
			this._relation = _legalRelations.RandomElement();

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.IsARelation".Translate(GetUniqueCombinedLabelFor(this._relation)) ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.IsARelation".Translate(GetUniqueCombinedLabelFor(this._relation)).StringSafe();
#else
			"PM.IsARelation".Translate(GetUniqueCombinedLabelFor(this._relation)).StringSafe;
#endif
	}
}
