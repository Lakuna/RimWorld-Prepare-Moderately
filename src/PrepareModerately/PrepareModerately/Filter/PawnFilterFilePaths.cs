using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

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

				IOrderedEnumerable<FileInfo> enumerable = directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				return new Collection<FileInfo>(enumerable.ToList());
#else
				return [.. enumerable];
#endif
			}
		}

		public static readonly string DataFolder = $"PawnFilters{Path.DirectorySeparatorChar}";

		public static string DataPath =>
			(PrepareModeratelyMod.Settings?.FilterSavePath is null || PrepareModeratelyMod.Settings.FilterSavePath.NullOrEmpty()) ? DefaultDataPath
			: PrepareModeratelyMod.Settings.FilterSavePath;

		public static string DefaultDataPath {
			get {
				Type type = typeof(GenFilePaths);
				string name = "FolderUnderSaveData";
				BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				return (string)type.GetMethod(name, flags)?.Invoke(null, new object[] { DataFolder }) ?? "null";
#else
				return (string?)type.GetMethod(name, flags)?.Invoke(null, [DataFolder]) ?? "null";
#endif
			}
		}

		public const string FileExtension = ".rpf";

		public static string AbsolutePathForName(string filterName) =>
			Path.Combine(DataPath, filterName + FileExtension);
	}
}
