using System;
using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDef`.
	public sealed class PawnFilterPartDef : Def {
		public PawnFilterPartCategory Category {
			get => this._category;
			set => this._category = value;
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public Type FilterPartClass {
#else
		public Type? FilterPartClass {
#endif
			get => this._filterPartClass;
			set => this._filterPartClass = value;
		}

		public int MaxUses {
			get => this._maxUses;
			set => this._maxUses = value;
		}

		public bool PlayerAddRemovable =>
			this.Category != PawnFilterPartCategory.Fixed;

		public float SelectionWeight {
			get => this._selectionWeight;
			set => this._selectionWeight = value;
		}

		public float SummaryPriority {
			get => this._summaryPriority;
			set => this._summaryPriority = value;
		}

		private PawnFilterPartCategory _category;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private Type _filterPartClass;
#else
		private Type? _filterPartClass;
#endif

		private int _maxUses = 999999;

		private float _selectionWeight = 1;

		private float _summaryPriority = 1;

		public override IEnumerable<string> ConfigErrors() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			foreach (string item in base.ConfigErrors() ?? new List<string>()) {
#else
			foreach (string item in base.ConfigErrors() ?? []) {
#endif
				yield return item;
			}

			if (this.FilterPartClass is null) {
				yield return "Filter part class is null.";
			}
		}
	}
}
