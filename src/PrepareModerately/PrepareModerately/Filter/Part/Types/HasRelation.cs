using System;
using System.Collections.Generic;
using System.Linq;

using Lakuna.PrepareModerately.UI;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasRelation : PawnFilterPart {
		private static string GetCombinedLabelFor(PawnRelationDef def) => def.labelFemale.NullOrEmpty() ? def.label : $"{def.label}/{def.labelFemale}";

		private static IEnumerable<PawnRelationDef> LegalRelations => DefDatabase<PawnRelationDef>.AllDefs;

		private static string GetUniqueCombinedLabelFor(PawnRelationDef def) =>
			GetCombinedLabelFor(def).NullOrEmpty() ? def.defName
			: LegalRelations.Count((def2) => GetCombinedLabelFor(def) == GetCombinedLabelFor(def2)) > 1 ? $"{GetCombinedLabelFor(def)} ({def.defName})"
			: GetCombinedLabelFor(def);

		private PawnRelationDef relation;

		/*
		public override bool Matches(Pawn pawn) => pawn is null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.relations.DirectRelations.Any((relation) => relation.def == this.relation);
		*/

		// TODO: Use commented version instead.
		public override bool Matches(Pawn pawn) {
			if (pawn is null) {
				throw new ArgumentNullException(nameof(pawn));
			}

			bool result = pawn.relations.DirectRelations.Any((relation) => relation.def == this.relation);

			string log = $"Checking match for pawn {pawn.Name.ToStringFull} with {GetUniqueCombinedLabelFor(this.relation)} ({result}):";
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations) {
				log += $"\n- {GetUniqueCombinedLabelFor(relation.def)} ({relation.def == this.relation})";
			}
			PrepareModeratelyLogger.LogMessage(log);

			return result;
		}

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, GetUniqueCombinedLabelFor(this.relation).CapitalizeFirst())) {
				FloatMenuUtility.MakeMenu(LegalRelations.OrderBy((def) => GetUniqueCombinedLabelFor(def)),
					(def) => GetUniqueCombinedLabelFor(def).CapitalizeFirst(),
					(def) => () => this.relation = def);
			}
		}

		public override string Summary(PawnFilter filter) => "PM.IsARelation".Translate(GetUniqueCombinedLabelFor(this.relation));

		public override void Randomize() => this.relation = LegalRelations.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.relation, nameof(this.relation));
		}

		public override bool CanCoexistWith(PawnFilterPart other) => other is null
			? throw new ArgumentNullException(nameof(other))
			: other.Def != PawnFilterPartDefOf.HasAnyRelation;
	}
}
