using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class HasAnyPermanentMedicalConditionFilterPart : FilterPart {
		public override bool Matches(Pawn pawn) {
			return pawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.IsPermanent() || hediff.def.chronic);
		}

		public override string Summary(Filter filter) {
			return "HasAnyPermanentMedicalConditions".Translate();
		}
	}
}
