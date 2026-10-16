using System;
using System.Collections.Generic;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPartDef`.
	public sealed class PawnFilterPartDef : Def {
		public PawnFilterPartCategory Category =>
			this._category;

		internal Type FilterPartClass =>
			this._filterPartClass;

		internal bool PlayerAddRemovable =>
			this.Category != PawnFilterPartCategory.Fixed;

		internal int MaxUses =>
			this._maxUses;

		internal float SelectionWeight =>
			this._selectionWeight;

		internal float SummaryPriority =>
			this._summaryPriority;

#pragma warning disable CS0649 // Value is assigned via XML definitions.
		private readonly PawnFilterPartCategory _category;
#pragma warning restore CS0649

#pragma warning disable CS0649 // Value is assigned via XML definitions.
		private readonly Type _filterPartClass;
#pragma warning restore CS0649

		private readonly int _maxUses;

		private readonly float _selectionWeight;

		private readonly float _summaryPriority;

		public PawnFilterPartDef() {
			this._summaryPriority = 1;
			this._selectionWeight = 1;
			this._maxUses = 999999;
		}

		public override IEnumerable<string> ConfigErrors() {
			foreach (string item in base.ConfigErrors()) {
				yield return item;
			}

			if (this.FilterPartClass is null) {
				yield return "Filter part class is null.";
			}
		}
	}
}
