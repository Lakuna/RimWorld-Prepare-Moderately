﻿using Lakuna.PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public class HasRelation : PawnFilterPart {
		private PawnRelationDef relation;

		public override bool Matches(Pawn pawn) => pawn == null
			? throw new ArgumentNullException(nameof(pawn))
			: pawn.relations.DirectRelations.Find((relation) => relation.def == this.relation) != null;

		public override void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			if (listing == null) {
				throw new ArgumentNullException(nameof(listing));
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight, out Rect rect);
			if (Widgets.ButtonText(rect, this.relation.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<PawnRelationDef>.AllDefsListForReading,
					(def) => def.LabelCap,
					(def) => () => this.relation = def);
			}
		}

		public override string Summary(PawnFilter filter) => "IsARelation".Translate(this.relation.label);

		public override void Randomize() => this.relation = DefDatabase<PawnRelationDef>.AllDefsListForReading.RandomElement();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.relation, nameof(this.relation));
		}

		public override bool CanCoexistWith(PawnFilterPart other) => other == null
			? throw new ArgumentNullException(nameof(other))
			: other.Def != PawnFilterPartDefOf.HasAnyRelation;
	}
}
