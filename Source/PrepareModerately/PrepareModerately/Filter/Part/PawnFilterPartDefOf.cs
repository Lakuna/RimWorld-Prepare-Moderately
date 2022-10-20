using RimWorld;

namespace Lakuna.PrepareModerately.Filter.Part {
	[DefOf]
	public class PawnFilterPartDefOf {
		// TODO: public static PawnFilterPartDef NameMatches; etc.

		static PawnFilterPartDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterPartDefOf));
	}
}
