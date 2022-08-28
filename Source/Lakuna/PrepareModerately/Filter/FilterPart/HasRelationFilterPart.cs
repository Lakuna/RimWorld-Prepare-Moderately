using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasRelationFilterPart : FilterPart {
		public PawnRelationDef relation;

		public override bool Matches(Pawn pawn) {
			return pawn.relations.DirectRelations.Find((DirectPawnRelation relation) => relation.def == this.relation) != null;
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, Text.LineHeight);
			if (Widgets.ButtonText(rect, this.relation.LabelCap)) {
				FloatMenuUtility.MakeMenu(DefDatabase<PawnRelationDef>.AllDefsListForReading,
					(PawnRelationDef def) => def.LabelCap,
					(PawnRelationDef def) => () => this.relation = def);
			}
		}

		public override string Summary(Filter filter) {
			return "IsARelation".Translate(this.relation.label);
		}

		public override void Randomize() {
			this.relation = FilterPart.GetRandomOfDef(new PawnRelationDef());
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Defs.Look(ref this.relation, nameof(this.relation));
		}

		public override bool CanCoexistWith(FilterPart other) {
			return other.def != FilterPartDefOf.HasAnyRelation;
		}
	}
}
