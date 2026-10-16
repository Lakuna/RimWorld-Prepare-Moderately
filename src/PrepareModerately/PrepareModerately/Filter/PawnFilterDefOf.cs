using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDefOf`.
	[DefOf]
	public static class PawnFilterDefOf {
#pragma warning disable IDE1006 // `DefOf` relies on static fields with names matching XML definitions.
		public static readonly PawnFilterDef Basic;

		public static readonly PawnFilterDef Empty;

		public static readonly PawnFilterDef Minimal;
#pragma warning restore IDE1006

		static PawnFilterDefOf() =>
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
