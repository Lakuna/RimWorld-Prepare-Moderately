using System.Linq;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class IsCapableOfEverythingFilterPart : FilterPart {

		public override bool Matches(Pawn pawn) {
			return DefDatabase<WorkTypeDef>.AllDefsListForReading.All((WorkTypeDef def) => !pawn.WorkTypeIsDisabled(def));
		}

		public override string Summary(Filter filter) {
			return "IsCapableOfEverything".Translate();
		}

		public override bool CanCoexistWith(FilterPart other) {
			return other.def != FilterPartDefOf.IsCapableOf;
		}
	}
}
