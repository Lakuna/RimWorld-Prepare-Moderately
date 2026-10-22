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

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static void LogException(Exception e, string description = "No description provided.", PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
#else
		public static void LogException(Exception? e, string description = "No description provided.", PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
#endif
			string output = $"{_prefix}{description}\n";

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Exception innerException = e;
#else
			Exception? innerException = e;
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			while (innerException != null) {
#else
			while (innerException is not null) {
#endif
				output += $"\n> {innerException.Message}";
				innerException = innerException.InnerException;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (e != null) {
#else
			if (e is not null) {
#endif
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
