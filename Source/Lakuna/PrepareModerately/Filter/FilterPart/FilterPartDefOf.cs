using RimWorld;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	[DefOf]
	public static class FilterPartDefOf {
		static FilterPartDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(FilterPartDefOf));
		}
	}
}
