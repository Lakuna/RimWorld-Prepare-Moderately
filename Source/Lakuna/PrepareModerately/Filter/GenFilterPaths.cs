using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public static class GenFilterPaths {
		public const string FilterExtension = ".rpf";

		private static string FiltersFolderPath => GenFilterPaths.FolderUnderSaveData("Filter");

		public static IEnumerable<FileInfo> AllCustomFilterFiles {
			get {
				DirectoryInfo directoryInfo = new DirectoryInfo(GenFilterPaths.FiltersFolderPath);
				if (!directoryInfo.Exists) { directoryInfo.Create(); }
				return from file in directoryInfo.GetFiles() where file.Extension == GenFilterPaths.FilterExtension orderby file.LastWriteTime descending select file;
			}
		}

		public static string FolderUnderSaveData(string folderName) {
			string path = Path.Combine(GenFilePaths.SaveDataFolderPath, folderName);
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists) { directoryInfo.Create(); }
			return path;
		}

		public static string AbsolutePathForFilter(string filterName) {
			return Path.Combine(GenFilterPaths.FiltersFolderPath, filterName + GenFilterPaths.FilterExtension);
		}
	}
}
