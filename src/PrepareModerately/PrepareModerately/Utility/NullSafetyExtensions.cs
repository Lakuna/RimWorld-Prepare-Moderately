using System;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
#pragma warning disable CA1708 // dotnet/sdk#51716 not backported to .NET 10.
	public static class NullSafetyExtensions {
#pragma warning restore CA1708
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static string CapitalizeAsTitleSafe(this string s) =>
			s is null ? "null"
			: (GenText.CapitalizeAsTitle(s) ?? s);

		public static string CapitalizeFirstSafe(this string s) =>
			s is null ? "null"
			: (s.CapitalizeFirst() ?? s);

		public static string CapitalizeSentencesSafe(this string s) =>
			s is null ? "null"
			: (GenText.CapitalizeSentences(s) ?? s);

		public static string EndWithPeriodSafe(this string s) =>
			s is null ? "null"
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			: s.EndsWith(".") ? s
			: $"{s}.";
#elif V1_5
			: (s.EndWithPeriod() ?? (s.EndsWith(".") ? s
					: $"{s}."));
#else
			: (s.EndWithPeriod() ?? (s.EndsWith('.') ? s
					: $"{s}."));
#endif

		public static string Finish(this string s, CapitalizeType capitalize = CapitalizeType.No, bool endWithPeriod = false) {
			if (s is null) {
				return "null";
			}

			string o = s;
			switch (capitalize) {
				case CapitalizeType.First:
					s = s.CapitalizeFirstSafe();
					break;
				case CapitalizeType.Sentences:
					s = s.CapitalizeSentencesSafe();
					break;
				case CapitalizeType.Title:
					s = s.CapitalizeAsTitleSafe();
					break;
			}

			if (endWithPeriod) {
				o = o.EndWithPeriodSafe();
			}

			return o;
		}

#if !V1_0
		public static string Finish(this TaggedString s, CapitalizeType capitalize = CapitalizeType.No, bool endWithPeriod = false) =>
			s.StringSafe().Finish(capitalize, endWithPeriod);
#endif

#if !V1_0
		public static string StringSafe(this TaggedString s) =>
			s.ToString() ?? "null"; // `ToString` doesn't strip tags like the implicit operator does.
#endif

		public static string TranslateFull(this string s, params NamedArgument[] args) =>
			s is null ? "null"
			: s.TranslateFull(CapitalizeType.No, args);

		public static string TranslateFull(this string s, CapitalizeType capitalize = CapitalizeType.No, params NamedArgument[] args) =>
			s is null ? "null"
			: s.TranslateFull(capitalize, false, args);

		public static string TranslateFull(this string s, CapitalizeType capitalize = CapitalizeType.No, bool endWithPeriod = false, params NamedArgument[] args) =>
			s is null ? "null"
#if V1_0
			: s.Translate(args).Finish(capitalize, endWithPeriod);
#else
			: s.Translate(args).StringSafe().Finish(capitalize, endWithPeriod);
#endif
#else
#pragma warning disable CA1034 // dotnet/sdk#51681 not backported to .NET 10.
		extension(string? s) {
#pragma warning restore CA1034
			public string CapitalizeAsTitleSafe =>
				s is null ? "null"
				: (GenText.CapitalizeAsTitle(s) ?? s);

			public string CapitalizeFirstSafe =>
				s is null ? "null"
				: (s.CapitalizeFirst() ?? s);

			public string CapitalizeSentencesSafe =>
				s is null ? "null"
				: (GenText.CapitalizeSentences(s) ?? s);

			public string EndWithPeriodSafe =>
				s is null ? "null"
				: (s.EndWithPeriod() ?? (s.EndsWith('.') ? s
						: $"{s}."));

			public string Finish(CapitalizeType capitalize = CapitalizeType.No, bool endWithPeriod = false) {
				if (s is null) {
					return "null";
				}

				string o = capitalize switch {
					CapitalizeType.First => s.CapitalizeFirstSafe,
					CapitalizeType.No => s,
					CapitalizeType.Sentences => s.CapitalizeSentencesSafe,
					CapitalizeType.Title => s.CapitalizeAsTitleSafe,
					_ => throw new NotImplementedException()
				};

				if (endWithPeriod) {
					o = o.EndWithPeriodSafe;
				}

				return o;
			}

			public string TranslateFull(params NamedArgument[] args) =>
				s is null ? "null"
				: s.TranslateFull(CapitalizeType.No, args);

			public string TranslateFull(CapitalizeType capitalize = CapitalizeType.No, params NamedArgument[] args) =>
				s is null ? "null"
				: s.TranslateFull(capitalize, false, args);

			public string TranslateFull(CapitalizeType capitalize = CapitalizeType.No, bool endWithPeriod = false, params NamedArgument[] args) =>
				s is null ? "null"
				: s.Translate(args).StringSafe.Finish(capitalize, endWithPeriod);
		}

#pragma warning disable CA1034 // dotnet/sdk#51681 not backported to .NET 10.
		extension(TaggedString s) {
#pragma warning restore CA1034
			public string StringSafe =>
				s.ToString() ?? "null"; // `ToString` doesn't strip tags like the implicit operator does.

			public string Finish(CapitalizeType capitalize = CapitalizeType.No, bool endWithPeriod = false) =>
				s.StringSafe.Finish(capitalize, endWithPeriod);
		}
#endif
	}
}
