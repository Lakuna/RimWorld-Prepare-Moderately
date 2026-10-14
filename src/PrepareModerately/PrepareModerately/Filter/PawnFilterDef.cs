using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDef`.
	public class PawnFilterDef : Def {
		public PawnFilter PFilter =>
			this.filter;

#pragma warning disable CS0649 // Value is assigned via XML definitions.
		private readonly PawnFilter filter;
#pragma warning restore CS0649

		public override IEnumerable<string> ConfigErrors() {
			if (this.PFilter is null) {
				yield return "Null filter.";
			}

			foreach (string item in this.PFilter.configErrors()) {
				yield return item;
			}
		}

		public override void PostLoad() {
			base.PostLoad();
			if (this.PFilter.PName.NullOrEmpty()) {
				this.PFilter.PName = this.label;
			}

			if (this.PFilter.PDescription.NullOrEmpty()) {
				this.PFilter.PDescription = this.description;
			}

			this.PFilter.PCategory = PawnFilterCategory.FromDef;
		}
	}
}
