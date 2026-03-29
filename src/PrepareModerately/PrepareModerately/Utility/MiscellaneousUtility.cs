using Verse;

namespace Lakuna.PrepareModerately.Utility {
	/// <summary>
	/// Miscellaneous utility methods.
	/// </summary>
	public static class MiscellaneousUtility {
		/// <summary>
		/// End the given string with a period.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <returns>The string.</returns>
		public static string EndWithPeriod(string s) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			$"{s}.";
#else
			s.EndWithPeriod();
#endif

#if !V1_0
		/// <summary>
		/// End the given tagged string with a period.
		/// </summary>
		/// <param name="s">The tagged string.</param>
		/// <returns>The tagged string.</returns>
		public static TaggedString EndWithPeriod(TaggedString s) =>
#if V1_1 || V1_2 || V1_3 || V1_4
			$"{s}.";
#else
			s.EndWithPeriod();
#endif
#endif
	}
}
