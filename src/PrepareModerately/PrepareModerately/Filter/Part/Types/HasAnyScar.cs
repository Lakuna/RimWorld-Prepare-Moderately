using System;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part.Types {
	[Obsolete("Prefer HasHediff.")]
	public sealed class HasAnyScar : PawnFilterPart {
		public override bool Matches(Pawn pawn) =>
			pawn.health?.hediffSet?.hediffs?.Any((hediff) => hediff?.IsPermanent() ?? false) ?? false;

		public override string Summary(PawnFilter filter) =>
			"PM.HasAnyScar".TranslateFull();
	}
}
