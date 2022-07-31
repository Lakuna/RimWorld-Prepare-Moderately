using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lakuna.PrepareModerately.PawnFilter;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately {
	// Based on VFECore.VFEGlobal from https://github.com/AndroidQuazar/VanillaExpandedFramework.
	public class PrepareModeratelyMod : Mod {
		// Based on Verse.Current.Game.Scenario.
		public static Filter currentFilter;

		// Based on Verse.Current.Game.Scenario.GetFirstConfigPage().
		public static Page_ConfigureStartingPawns page_ConfigureStartingPawns;

		// Based on Verse.GenFilePaths.ScenarioExtension.
		public const string FilterExtension = ".rpf";

		// Based on Verse.GenFilePaths.ScenariosFolderPath.
		private static string FiltersFolderPath => (string)typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { "PrepareModerately" });

		// Based on Verse.GenFilePaths.AllCustomScenarioFiles.
		public static IEnumerable<FileInfo> AllCustomFilterFiles {
			get {
				DirectoryInfo directoryInfo = new DirectoryInfo(PrepareModeratelyMod.FiltersFolderPath);
				if (!directoryInfo.Exists) { directoryInfo.Create(); }
				return from f in directoryInfo.GetFiles() where f.Extension == PrepareModeratelyMod.FilterExtension orderby f.LastWriteTime descending select f;
			}
		}

		// Based on Verse.GenFilePaths.AbsPathForScenario.
		public static string AbsolutePathForFilter(string filterName) {
			return Path.Combine(PrepareModeratelyMod.FiltersFolderPath, filterName + PrepareModeratelyMod.FilterExtension);
		}
	}
}

// Note: look at Verse.PawnGenerationRequest. An example is in RimWorld.ScenPart_PawnFilter_Age.AllowPlayerStartingPawn.
