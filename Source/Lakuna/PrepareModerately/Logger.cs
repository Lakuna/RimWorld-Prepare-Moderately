using System;
using Verse;

namespace Lakuna.PrepareModerately {
	public static class Logger {
		public static void LogException(Exception e, string description = "no description provided") {
			string output = "Prepare Moderately encountered an exception: " + description + ".";

			Exception innerException = e;
			while (innerException != null) {
				output += "\n>" + innerException.Message;
				innerException = innerException.InnerException;
			}

			output += "\n\n" + "StackTrace".Translate() + "\n" + e.StackTrace + "\n\n";

			Log.Error(output);
		}
	}
}
