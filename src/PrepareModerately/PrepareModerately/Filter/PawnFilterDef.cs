using System.Collections.Generic;

using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.ScenarioDef`.
	public sealed class PawnFilterDef : Def {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public PawnFilter Filter {
#else
		public PawnFilter? Filter {
#endif
			get => this._filter;
			set => this._filter = value;
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private PawnFilter _filter;
#else
		private PawnFilter? _filter;
#endif

		public override IEnumerable<string> ConfigErrors() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			foreach (string item in base.ConfigErrors() ?? new List<string>()) {
#else
			foreach (string item in base.ConfigErrors() ?? []) {
#endif
				yield return item;
			}

			if (this.Filter is null) {
				yield return "Pawn filter definition has null filter.";
			} else {
				foreach (string item in this.Filter.ConfigErrors) {
					yield return item;
				}
			}
		}

		public override void PostLoad() {
			base.PostLoad();

			if (this.Filter is null) {
				PrepareModeratelyLogger.LogErrorMessage("Loaded pawn filter definition with null pawn filter.");
				return;
			}

			if (this.Filter.Name is null || this.Filter.Name.NullOrEmpty()) {
				this.Filter.Name = this.label ?? "null";
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (this.Filter.Description is null) {
				this.Filter.Description = this.description ?? "null";
			}
#else
			this.Filter.Description ??= this.description ?? "null";
#endif

			this.Filter.Category = PawnFilterCategory.FromDef;
		}
	}
}
