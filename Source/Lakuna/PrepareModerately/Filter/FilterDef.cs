using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public class FilterDef : Def {
		public Filter filter;

		public override void PostLoad() {
			base.PostLoad();

			if (this.filter.name.NullOrEmpty()) { this.filter.name = this.label; }
			if (this.filter.description.NullOrEmpty()) { this.filter.description = this.description; }
			this.filter.Category = FilterCategory.FromDef;
		}

		public override IEnumerable<string> ConfigErrors() {
			if (this.filter == null) { yield return "Null filter."; }

			foreach (string item in this.filter.ConfigErrors()) {
				yield return item;
			}
		}
	}
}
