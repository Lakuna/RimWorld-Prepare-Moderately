using System;
using Lakuna.PrepareModerately.Filter;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	public class FilterListLoadDialog : FilterListDialog {
		private Action<Filter.Filter> filterReturner;

		public FilterListLoadDialog(Action<Filter.Filter> filterReturner) {
			this.interactButLabel = "LoadGameButton".Translate();
			this.filterReturner = filterReturner;
		}

		protected override void DoFileInteraction(string fileName) {
			string filePath = GenFilterPaths.AbsolutePathForFilter(fileName);
			PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, delegate {
				Filter.Filter filter = null;
				if (FilterSaveLoader.TryLoadFilter(filePath, FilterCategory.CustomLocal, out filter)) {
					filterReturner(filter);
				}
				this.Close();
			});
		}
	}
}
