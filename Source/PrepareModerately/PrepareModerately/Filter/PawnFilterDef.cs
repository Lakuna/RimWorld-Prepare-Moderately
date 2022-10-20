using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public class PawnFilterDef : Def {
		public PawnFilter Filter { get; }

		public override void PostLoad() {
			base.PostLoad();
			if (this.Filter.Name.NullOrEmpty()) { this.Filter.Name = this.label; }
			if (this.Filter.Description.NullOrEmpty()) { this.Filter.Description = this.description; }
			this.Filter.Category = PawnFilterCategory.FromDef;
		}

		public override IEnumerable<string> ConfigErrors() {
			if (this.Filter == null) { yield return "Null filter."; }

			foreach (string item in this.Filter.ConfigErrors()) {
				yield return item;
			}
		}
	}
}
