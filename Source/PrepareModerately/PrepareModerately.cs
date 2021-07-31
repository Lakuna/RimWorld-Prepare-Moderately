using PrepareModerately.UI;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately : Mod {
		public const string filterExtension = ".v2.xml";

		public static readonly string dataPath = (string) typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { "PrepareModerately" });

		public static void LogError(Exception e) {
			string output = "Prepare Moderately encountered an exception.";
			Exception innerException = e;
			while (innerException != null) {
				output += "\n>" + innerException.Message;
				innerException = innerException.InnerException;
			}
			output += "\n\nStack trace:\n" + e.StackTrace + "\n\n";
			Log.Error(output);
		}

		public static Settings settings;
		public static Pawn activePawn;
		public static bool activelyRolling = false;
		public static Page_PrepareModerately page;

		public PrepareModerately(ModContentPack content) : base(content) => PrepareModerately.settings = this.GetSettings<Settings>();

		public override void DoSettingsWindowContents(Rect rect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(rect);

			listing.Label("Pawns to generate per roll step (" + PrepareModerately.settings.rollMultiplier + ")");
			PrepareModerately.settings.rollMultiplier = (int) listing.Slider(PrepareModerately.settings.rollMultiplier, 1, 10);

			listing.Label("Roll steps between pawn generation (" + PrepareModerately.settings.rollModulus + ")");
			PrepareModerately.settings.rollModulus = (int) listing.Slider(PrepareModerately.settings.rollModulus, 1, 1000);

			listing.End();

			base.DoSettingsWindowContents(rect);
		}

		public override string SettingsCategory() => "Prepare Moderately";
	}
}
