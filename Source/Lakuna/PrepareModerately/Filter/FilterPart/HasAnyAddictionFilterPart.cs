using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAnyAddictionFilterPart : FilterPart {
		public override bool Matches(Pawn pawn) {
			return pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.def.IsAddiction);
		}

		public override void DoEditInterface(FilterEditListing listing) {
			listing.GetFilterPartRect(this, 0);
		}

		public override string Summary(Filter filter) {
			return "Has any addiction.";
		}
	}
}
