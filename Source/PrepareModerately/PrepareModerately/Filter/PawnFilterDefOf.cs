using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	[DefOf]
	public static class PawnFilterDefOf {
		public static readonly PawnFilterDef Empty;

		// TODO: Basic filter.

		static PawnFilterDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
