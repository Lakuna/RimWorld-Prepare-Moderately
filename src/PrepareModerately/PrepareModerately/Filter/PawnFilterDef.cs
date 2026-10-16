using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDef`.
	public sealed class PawnFilterDef : Def {
#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		public readonly PawnFilter filter;
#pragma warning restore CA1051, IDE1006

		public override IEnumerable<string> ConfigErrors() {
			if (this.filter is null) {
				yield return "Null filter.";
			}

			foreach (string item in this.filter.ConfigErrors()) {
				yield return item;
			}
		}

		public override void PostLoad() {
			base.PostLoad();
			if (this.filter.Name.NullOrEmpty()) {
				this.filter.Name = this.label;
			}

			if (this.filter.Description.NullOrEmpty()) {
				this.filter.Description = this.description;
			}

			this.filter.Category = PawnFilterCategory.FromDef;
		}
	}
}
