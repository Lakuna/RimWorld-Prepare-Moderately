using System;
using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public class FilterPartDef : Def {
		public FilterPartCategory category;

		public Type filterPartClass;

		public float summaryPriority;

		public float selectionWeight;

		public int maxUses;

		public bool PlayerAddRemovable => this.category != FilterPartCategory.Fixed;

		public override IEnumerable<string> ConfigErrors() {
			foreach (string item in base.ConfigErrors()) {
				yield return item;
			}

			if (this.filterPartClass == null) {
				yield return "filterPartClass is null";
			}
		}

		public FilterPartDef() {
			this.summaryPriority = -1;
			this.selectionWeight = -1;
			this.maxUses = 999999;
		}
	}
}
