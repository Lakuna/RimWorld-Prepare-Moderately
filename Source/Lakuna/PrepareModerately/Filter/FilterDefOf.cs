using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	[DefOf]
	public static class FilterDefOf {
		public static FilterDef Empty;

		static FilterDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(FilterDefOf));
		}
	}
}
