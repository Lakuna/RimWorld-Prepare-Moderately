using System;
using Verse;

namespace Lakuna.PrepareModerately.Utility {
	public static class PrepareModeratelyLogger {
		public static void LogException(Exception e, string description = "No description provided.",
			PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
			if (e == null) {
				throw new ArgumentNullException(nameof(e));
			}

			string output = "Prepare Moderately encountered an exception: " + description + "\n";

			Exception innerException = e;
			while (innerException != null) {
				output += "\n> " + innerException.Message;
				innerException = innerException.InnerException;
			}

			output += "\n\nStack trace:\n" + e.StackTrace + "\n\n";

			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error(output);
			} else {
				Log.ErrorOnce(output, (int)category);
			}
		}

		public static void LogErrorMessage(string e, PrepareModeratelyLoggerCategory category = PrepareModeratelyLoggerCategory.Unrestricted) {
			e = "Prepare Moderately encountered an issue: " + e;
			if (category == PrepareModeratelyLoggerCategory.Unrestricted) {
				Log.Error(e);
			} else {
				Log.ErrorOnce(e, (int)category);
			}
		}
	}
}
