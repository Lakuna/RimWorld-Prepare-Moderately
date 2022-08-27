using System;
using System.Linq;
using Lakuna.PrepareModerately.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsCapableOfEverythingFilterPart : FilterPart {

		public override bool Matches(Pawn pawn) {
			return DefDatabase<WorkTypeDef>.AllDefsListForReading.All((WorkTypeDef def) => !pawn.WorkTypeIsDisabled(def));
		}

		public override void DoEditInterface(FilterEditListing listing) {
			Rect rect = listing.GetFilterPartRect(this, 0);
		}

		public override string Summary(Filter filter) {
			return "Is capable of everything.";
		}

		public override bool CanCoexistWith(FilterPart other) {
			return other.def != FilterPartDefOf.IsCapableOf;
		}
	}
}
