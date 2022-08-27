using Lakuna.PrepareModerately.UI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAnyPermanentMedicalConditionFilterPart : FilterPart {
		public override bool Matches(Pawn pawn) {
			return pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.IsPermanent() || hediff.def.chronic);
		}

		public override void DoEditInterface(FilterEditListing listing) {
			listing.GetFilterPartRect(this, 0);
		}

		public override string Summary(Filter filter) {
			return "HasAnyPermanentMedicalConditions".Translate();
		}
	}
}
