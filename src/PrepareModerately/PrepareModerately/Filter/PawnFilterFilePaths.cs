using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `Verse.GenFilePaths`.
	public static class PawnFilterFilePaths {
		public static Collection<FileInfo> All {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				DirectoryInfo directoryInfo = new DirectoryInfo(DataPath);
#else
				DirectoryInfo directoryInfo = new(DataPath);
#endif
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				return new Collection<FileInfo>(directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime)
					.ToList());
#else
				return [.. directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime)];
#endif
			}
		}

		public static readonly string DataFolder = $"PawnFilters{Path.DirectorySeparatorChar}";

		public static string DataPath =>
			(PrepareModeratelyMod.Settings?.FilterSavePath is null || PrepareModeratelyMod.Settings.FilterSavePath.NullOrEmpty()) ? DefaultDataPath
			: PrepareModeratelyMod.Settings.FilterSavePath;

		public static string DefaultDataPath =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			((string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				?.Invoke(null, new object[] { DataFolder })) ?? "";
#else
			((string?)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				?.Invoke(null, [DataFolder])) ?? "";
#endif

		public const string FileExtension = ".rpf";

		public static string AbsolutePathForName(string filterName) =>
			Path.Combine(DataPath, filterName + FileExtension);
	}
}
