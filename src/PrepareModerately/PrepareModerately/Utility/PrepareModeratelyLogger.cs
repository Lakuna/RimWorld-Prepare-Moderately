using System;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
	internal static class PrepareModeratelyLogger {
		private const int _base = 31303;

		private const string _prefix = "Prepare Moderately encountered an exception: ";

		internal static void LogErrorMessage(string e, PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error($"{_prefix}{e}");
				return;
			}

			Log.ErrorOnce($"{_prefix}{e}", _base + (int)category);
		}

		internal static void LogException(Exception e, string description = "No description provided.", PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (e is null) {
				throw new ArgumentNullException(nameof(e));
			}
#else
			ArgumentNullException.ThrowIfNull(e);
#endif

			string output = $"{_prefix}{description}\n";

			Exception innerException = e;
			while (innerException != null) {
				output += $"\n> {innerException.Message}";
				innerException = innerException.InnerException;
			}

			output += $"\n\nStack trace:\n{e.StackTrace}\n\n";

			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error(output);
				return;
			}

			Log.ErrorOnce(output, _base + (int)category);
		}

		internal static void LogMessage(string message) => Log.Message(message);
	}
}
