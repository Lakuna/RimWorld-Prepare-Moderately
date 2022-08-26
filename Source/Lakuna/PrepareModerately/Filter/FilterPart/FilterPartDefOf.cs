using RimWorld;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	[DefOf]
	public static class FilterPartDefOf {
		public static FilterPartDef NameContains;

		static FilterPartDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(FilterPartDefOf));
		}
	}
}
