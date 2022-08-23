using System.Collections.Generic;
using System.IO;

namespace Lakuna.PrepareModerately.Filter {
	public static class FilterFiles {
		private static List<Filter> filtersLocal;

		public static IEnumerable<Filter> AllFiltersLocal => FilterFiles.filtersLocal;

		public static void RecacheData() {
			filtersLocal.Clear();
			foreach (FileInfo customFilterFile in GenFilterPaths.AllCustomFilterFiles) {
				if (FilterSaveLoader.TryLoadFilter(customFilterFile.FullName, FilterCategory.CustomLocal, out Filter filter)) { FilterFiles.filtersLocal.Add(filter); }
			}
		}

		static FilterFiles() {
			FilterFiles.filtersLocal = new List<Filter>();
		}
	}
}
