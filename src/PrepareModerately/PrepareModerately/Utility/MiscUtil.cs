using System;

namespace Lakuna.PrepareModerately.Utility {
	internal static class MiscUtil {
		internal static string NoUnder(string s) =>
			s is null ? throw new ArgumentNullException(nameof(s))
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			: s.StartsWith("_") ? s.Substring(1) : s;
#else
			: s.StartsWith('_') ? s[1..] : s;
#endif
	}
}
