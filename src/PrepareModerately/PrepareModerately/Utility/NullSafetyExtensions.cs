using Verse;

namespace Lakuna.PrepareModerately.Utility {
#pragma warning disable CA1708 // dotnet/sdk#51716 not backported to .NET 10.
	public static class NullSafetyExtensions {
#pragma warning restore CA1708
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static string CapitalizeAsTitleSafe(this string s) =>
			GenText.CapitalizeAsTitle(s) ?? s ?? "null";

		public static string CapitalizeFirstSafe(this string s) =>
			s.CapitalizeFirst() ?? s ?? "null";

		public static string CapitalizeSentencesSafe(this string s) =>
			GenText.CapitalizeSentences(s) ?? s ?? "null";

#if !V1_0
		public static TaggedString EndWithPeriod(this TaggedString s) =>
			$"{s}.";
#endif

		public static string EndWithPeriodSafe(this string s) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			$"{s ?? "null"}.";
#else
			s.EndWithPeriod() ?? $"{s ?? "null"}.";
#endif

#if !V1_0
		public static string StringSafe(this TaggedString s) {
			string o = (string)s; // Implicit operator is preferable to `ToString` because it strips tags.
			return o.NullOrEmpty() ? (s.ToString() ?? "null") : o;
		}
#endif
#else
#pragma warning disable CA1034 // dotnet/sdk#51681 not backported to .NET 10.
		extension(string? s) {
#pragma warning restore CA1034
			public string CapitalizeAsTitleSafe =>
				GenText.CapitalizeAsTitle(s) ?? s ?? "null";

			public string CapitalizeFirstSafe =>
				s.CapitalizeFirst() ?? s ?? "null";

			public string CapitalizeSentencesSafe =>
				GenText.CapitalizeSentences(s) ?? s ?? "null";

			public string EndWithPeriodSafe =>
				s.EndWithPeriod() ?? $"{s ?? "null"}.";
		}

#pragma warning disable CA1034 // dotnet/sdk#51681 not backported to .NET 10.
		extension(TaggedString s) {
#pragma warning restore CA1034
			public string StringSafe {
				get {
					string o = (string)s; // Implicit operator is preferable to `ToString` because it strips tags.
					return o.NullOrEmpty() ? (s.ToString() ?? "null") : o;
				}
			}
		}
#endif
	}
}
