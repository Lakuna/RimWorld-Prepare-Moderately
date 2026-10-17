using System;

using Verse;

namespace Lakuna.PrepareModerately.Utility {
	internal static class PrepareModeratelyLogger {
		private const int _keyBase = 31303;

		private const string _prefix = "Prepare Moderately encountered an exception: ";

		internal static void LogErrorMessage(string e, PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error($"{_prefix}{e}");
				return;
			}

			Log.ErrorOnce($"{_prefix}{e}", _keyBase + (int)category);
		}

		internal static void LogException(Exception e, string description = "No description provided.", PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
			if (e is null) {
				Log.Error($"{_prefix}null.");
				return;
			}

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

			Log.ErrorOnce(output, _keyBase + (int)category);
		}

		internal static void LogMessage(string message) => Log.Message(message);
	}
}
