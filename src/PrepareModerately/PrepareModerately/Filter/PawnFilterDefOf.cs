using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDefOf`.
	[DefOf]
	internal static class PawnFilterDefOf {
#pragma warning disable CS0649, IDE1006 // `DefOf` relies on static fields with names matching XML definitions.
		internal static readonly PawnFilterDef Basic;

		internal static readonly PawnFilterDef Empty;

		internal static readonly PawnFilterDef Minimal;
#pragma warning restore CS0649, IDE1006

		static PawnFilterDefOf() =>
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
