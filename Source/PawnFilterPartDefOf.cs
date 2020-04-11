using RimWorld;

namespace PrepareModerately {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static PawnFilterPartDef NameStartsWith;

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
