using System;
using Verse;

namespace Lakuna.PrepareModerately {
	public static class Logger {
		public enum LoggerCategory {
			Unrestricted,
			GetFullInformationText
		}

		public static void LogException(Exception e, string description = "No description provided.", LoggerCategory category = LoggerCategory.Unrestricted) {
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

			if (category == LoggerCategory.Unrestricted) {
				Log.Error(output);
			} else {
				Log.ErrorOnce(output, (int)category);
			}
		}

		public static void LogErrorMessage(string e, LoggerCategory category = LoggerCategory.Unrestricted) {
			if (category == LoggerCategory.Unrestricted) {
				Log.Error(e);
			} else {
				Log.ErrorOnce(e, (int)category);
			}
		}
	}
}
