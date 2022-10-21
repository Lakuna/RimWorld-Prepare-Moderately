using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	[DefOf]
	public static class PawnFilterDefOf {
		public static readonly PawnFilterDef Empty;

		public static readonly PawnFilterDef Basic;

		static PawnFilterDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
