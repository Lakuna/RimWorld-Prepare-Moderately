using RimWorld;

namespace Lakuna.PrepareModerately.Filter {
	[DefOf]
	public class PawnFilterDefOf {
		// TODO: public static PawnFilterDef Empty; etc.

		static PawnFilterDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PawnFilterDefOf));
	}
}
