using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDef`.
	public sealed class PawnFilterDef : Def {
		public PawnFilter PFilter =>
			this._filter;

#pragma warning disable CS0649 // Value is assigned via XML definitions.
		private readonly PawnFilter _filter;
#pragma warning restore CS0649

		public override IEnumerable<string> ConfigErrors() {
			if (this.PFilter is null) {
				yield return "Null filter.";
			}

			foreach (string item in this.PFilter.ConfigErrors()) {
				yield return item;
			}
		}

		public override void PostLoad() {
			base.PostLoad();
			if (this.PFilter.Name.NullOrEmpty()) {
				this.PFilter.Name = this.label;
			}

			if (this.PFilter.Description.NullOrEmpty()) {
				this.PFilter.Description = this.description;
			}

			this.PFilter.Category = PawnFilterCategory.FromDef;
		}
	}
}
