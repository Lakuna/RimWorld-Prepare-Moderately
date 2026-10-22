#if !V1_0
using Lakuna.PrepareModerately.Utility;
#endif

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	public sealed class HasAnyMissingBodyPart : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff is Hediff_MissingPart) ?? false;

		public override string Summary(PawnFilter filter) =>
#if V1_0
			"PM.HasAnyMissingBodyPart".Translate() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.HasAnyMissingBodyPart".Translate().StringSafe();
#else
			"PM.HasAnyMissingBodyPart".Translate().StringSafe;
#endif
	}
}
