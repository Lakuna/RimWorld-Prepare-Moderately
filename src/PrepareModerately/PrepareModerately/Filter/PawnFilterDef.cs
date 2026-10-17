using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDef`.
#pragma warning disable CA1812 // Class is instantiated via RimWorld.
	internal sealed class PawnFilterDef : Def {
#pragma warning restore CA1812
#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		internal readonly PawnFilter filter;
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
			if (this.filter.name.NullOrEmpty()) {
				this.filter.name = this.label;
			}

			if (this.filter.description.NullOrEmpty()) {
				this.filter.description = this.description;
			}

			this.filter._category = PawnFilterCategory.FromDef;
		}
	}
}
