using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDefOf`.
	[DefOf]
	public static class PawnFilterDefOf {
		public static readonly PawnFilterDef Empty;

		public static readonly PawnFilterDef Basic;

		public static readonly PawnFilterDef Minimal;

		static PawnFilterDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
