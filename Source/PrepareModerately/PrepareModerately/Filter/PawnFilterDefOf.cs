using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	[DefOf]
	public static class PawnFilterDefOf {
		public static PawnFilterDef Empty { get; }

		public static PawnFilterDef Basic { get; }

		static PawnFilterDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
