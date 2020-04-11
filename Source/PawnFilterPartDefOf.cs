using RimWorld;

namespace PrepareModerately {
	[DefOf]
	public static class PawnFilterPartDefOf {
		public static PawnFilterPartDef NameStartsWith;
		public static PawnFilterPartDef CapableOf;

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
