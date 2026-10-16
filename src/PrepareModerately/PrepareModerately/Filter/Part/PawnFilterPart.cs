using System;
using System.Collections.Generic;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPart`.
	public abstract class PawnFilterPart : IExposable {
		public virtual string Label =>
			this._def.label;

		public virtual PawnFilterPartDef Def {
			get => this._def;
			set => this._def = value;
		}

		internal virtual PawnFilterPart CopyForEditing =>
			(PawnFilterPart)this.MemberwiseClone();

		internal virtual bool Summarized {
			get; set;
		}

		internal virtual bool Visible =>
			true;

		[TranslationHandle]
		private PawnFilterPartDef _def;

		public virtual void ExposeData() =>
			Scribe_Defs.Look(ref this._def, MiscUtil.NoUnder(nameof(this._def)));

		public override int GetHashCode() =>
			this._def is null ? 0
			: this._def.GetHashCode();

		public virtual bool Matches(Pawn pawn) =>
			true;

		public virtual bool NotMatches(Pawn pawn) =>
			!this.Matches(pawn);

		public virtual string Summary(PawnFilter filter) =>
			this._def.description;

		internal virtual bool CanCoexistWith(PawnFilterPart other) =>
			true;

		internal virtual IEnumerable<string> ConfigErrors() {
			if (this._def is null) {
				yield return $"{this.GetType()} has a null definition.".CapitalizeFirst();
			}
		}

		internal virtual void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (listing is null) {
				throw new ArgumentNullException(nameof(listing));
			}
#else
			ArgumentNullException.ThrowIfNull(listing);
#endif

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight);
		}

		internal virtual bool HasNullDefs() =>
			this._def is null;

		internal virtual void PreConfigure() {
		}

		internal virtual void Randomize() {
		}

		internal virtual bool TryMerge(PawnFilterPart other) =>
			false;

		internal virtual bool IsValid() =>
			!this.HasNullDefs();
	}
}
