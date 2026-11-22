using System;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
	public static class PrepareModeratelyLogger {
		private const int _keyBase = 79246141;

		private const string _prefix = "Prepare Moderately encountered an exception: ";

		public static void LogErrorMessage(string e, PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error($"{_prefix}{e}");
				return;
			}

			Log.ErrorOnce($"{_prefix}{e}", _keyBase + (int)category);
		}

		public static void LogException(
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Exception e,
#else
			Exception? e,
#endif
			string description = "No description provided.",
			PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted
		) {
			string output = $"{_prefix}{description}\n";

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Exception innerException = e;
#else
			Exception? innerException = e;
#endif
			while (
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				!(innerException is null)
#else
				innerException is not null
#endif
			) {
				output += $"\n> {innerException.Message}";
				innerException = innerException.InnerException;
			}

			if (
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				!(e is null)
#else
				e is not null
#endif
			) {
				output += $"\n\nStack trace:\n{e.StackTrace}\n\n";
			}

			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error(output);
				return;
			}

			Log.ErrorOnce(output, _keyBase + (int)category);
		}

		public static void LogMessage(string message) =>
			Log.Message(message);
	}
}
