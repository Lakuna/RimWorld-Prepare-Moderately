using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDefOf`.
	[DefOf]
	public static class PawnFilterDefOf {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterDef Basic;
#else
		public static readonly PawnFilterDef? Basic;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterDef Empty;
#else
		public static readonly PawnFilterDef? Empty;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly PawnFilterDef Minimal;
#else
		public static readonly PawnFilterDef? Minimal;
#endif

		static PawnFilterDefOf() =>
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
