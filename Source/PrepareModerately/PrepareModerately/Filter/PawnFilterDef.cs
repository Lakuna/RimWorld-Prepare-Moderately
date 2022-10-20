using System.Collections.Generic;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public class PawnFilterDef : Def {
#pragma warning disable CA1051 // Definitions loaded from XML files must use instance fields.
		public readonly PawnFilter filter;
#pragma warning restore CA1051

		public override void PostLoad() {
			base.PostLoad();
			if (this.filter.Name.NullOrEmpty()) { this.filter.Name = this.label; }
			if (this.filter.Description.NullOrEmpty()) { this.filter.Description = this.description; }
			this.filter.Category = PawnFilterCategory.FromDef;
		}

		public override IEnumerable<string> ConfigErrors() {
			if (this.filter == null) { yield return "Null filter."; }

			foreach (string item in this.filter.ConfigErrors()) {
				yield return item;
			}
		}
	}
}
