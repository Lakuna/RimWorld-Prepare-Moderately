using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDef`.
	public class PawnFilterDef : Def {
#pragma warning disable CA1051 // Definitions loaded from XML files must use instance fields.
		public readonly PawnFilter filter;
#pragma warning restore CA1051

		public override void PostLoad() {
			base.PostLoad();
			if (this.filter.PName.NullOrEmpty()) {
				this.filter.PName = this.label;
			}

			if (this.filter.PDescription.NullOrEmpty()) {
				this.filter.PDescription = this.description;
			}

			this.filter.PCategory = PawnFilterCategory.FromDef;
		}

		public override IEnumerable<string> ConfigErrors() {
			if (this.filter is null) {
				yield return "Null filter.";
			}

			foreach (string item in this.filter.ConfigErrors()) {
				yield return item;
			}
		}
	}
}
