using System.Collections.Generic;
using System.IO;
using System.Linq;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `Verse.GenFilePaths`.
	internal static class PawnFilterFilePaths {
		internal static IList<FileInfo> _allFiles {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				DirectoryInfo directoryInfo = new DirectoryInfo(_dataPath);
#else
				DirectoryInfo directoryInfo = new(_dataPath);
#endif
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				return directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == _fileExtension)
					.OrderByDescending((file) => file.LastWriteTime).ToList();
#else
				return [.. directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == _fileExtension)
					.OrderByDescending((file) => file.LastWriteTime)];
#endif
			}
		}

		private static readonly string _dataFolder = $"PawnFilters{Path.DirectorySeparatorChar}";

		private static string _dataPath =>
			PrepareModeratelyMod._settings.filterSavePath.NullOrEmpty() ? _defaultDataPath
			: PrepareModeratelyMod._settings.filterSavePath;

		internal static string _defaultDataPath =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, new object[] { _dataFolder });
#else
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, [_dataFolder]);
#endif

		private const string _fileExtension = ".rpf";

		internal static string AbsolutePathForName(string filterName) =>
			Path.Combine(_dataPath, filterName + _fileExtension);
	}
}
