using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	[DefOf]
	public static class FilterDefOf {
		public static FilterDef Empty;

		public static FilterDef Basic;

		static FilterDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(FilterDefOf));
		}
	}
}
