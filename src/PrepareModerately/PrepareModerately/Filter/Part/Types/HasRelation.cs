using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.Ui;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
#pragma warning disable CA1812 // Class is instantiated via XML definitions.
	internal sealed class HasRelation : PawnFilterPart {
#pragma warning restore CA1812
		private static IEnumerable<PawnRelationDef> LegalRelations =>
			DefDatabase<PawnRelationDef>.AllDefs;

#pragma warning disable IDE1006 // `Def` relies on static fields with names matching XML definitions.
		private PawnRelationDef relation;
#pragma warning restore IDE1006

		private static string GetCombinedLabelFor(PawnRelationDef def) =>
			def is null ? throw new ArgumentNullException(nameof(def))
			: def.labelFemale.NullOrEmpty() ? def.label : $"{def.label}/{def.labelFemale}";

		private static string GetUniqueCombinedLabelFor(PawnRelationDef def) =>
			GetCombinedLabelFor(def).NullOrEmpty() ? def.defName
			: LegalRelations.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName})"
			: GetCombinedLabelFor(def);

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.relation, nameof(this.relation));
		}

		public override bool Matches(Pawn pawn) =>
			pawn is null ? throw new ArgumentNullException(nameof(pawn))
#if V1_0 || V1_1 || V1_2
			: pawn.relations.RelatedPawns.Any((relatedPawn) =>
				(!relatedPawn.RaceProps.Animal || !relatedPawn.Dead || relatedPawn.Corpse != null)
				&& relatedPawn.Name != null
				&& !relatedPawn.Name.Numerical
				&& relatedPawn.relations.everSeenByPlayer
				&& pawn.GetRelations(relatedPawn).Any((relation) => relation == this.relation));
#else
			: !pawn.relations.hidePawnRelations &&
				pawn.relations.RelatedPawns.Any((relatedPawn) =>
				(!relatedPawn.RaceProps.Animal || !relatedPawn.Dead || relatedPawn.Corpse != null)
				&& relatedPawn.Name != null
				&& !relatedPawn.Name.Numerical
				&& !relatedPawn.relations.hidePawnRelations
				&& relatedPawn.relations.everSeenByPlayer
				&& pawn.GetRelations(relatedPawn).Any((relation) => relation == this.relation));
#endif

		public override string Summary(PawnFilter filter) =>
			"PM.IsARelation".Translate(GetUniqueCombinedLabelFor(this.relation));

		internal override bool CanCoexistWith(PawnFilterPart other) =>
			other is null ? throw new ArgumentNullException(nameof(other))
			: other.def != PawnFilterPartDefOf.HasAnyRelation;

		internal override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this.relation).CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalRelations.OrderBy(GetUniqueCombinedLabelFor),
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirst(),
					(def) => () => this.relation = def);
			}
		}

		internal override void Randomize() =>
			this.relation = LegalRelations.RandomElement();
	}
}
