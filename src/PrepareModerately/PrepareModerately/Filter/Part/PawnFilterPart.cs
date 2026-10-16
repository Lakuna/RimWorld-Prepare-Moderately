using System;
using System.Collections.Generic;

using Lakuna.PrepareModerately.Ui;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPart`.
	internal abstract class PawnFilterPart : IExposable {
		internal virtual string _label =>
			this.def.label;

		internal virtual PawnFilterPart _copyForEditing =>
			(PawnFilterPart)this.MemberwiseClone();

		internal virtual bool _summarized {
			get; set;
		}

		internal virtual bool _visible =>
			true;

#pragma warning disable CA1051, IDE1006 // `Def` relies on static fields with names matching XML definitions.
		[TranslationHandle]
		internal PawnFilterPartDef def;
#pragma warning restore CA1051, IDE1006

		public virtual void ExposeData() =>
			Scribe_Defs.Look(ref this.def, nameof(this.def));

		public override int GetHashCode() =>
			this.def is null ? 0
			: this.def.GetHashCode();

		internal virtual bool Matches(Pawn pawn) =>
			true;

		internal virtual bool NotMatches(Pawn pawn) =>
			!this.Matches(pawn);

		internal virtual string Summary(PawnFilter filter) =>
			this.def.description;

		internal virtual bool CanCoexistWith(PawnFilterPart other) =>
			true;

		internal virtual IEnumerable<string> ConfigErrors() {
			if (this.def is null) {
				yield return $"{this.GetType()} has a null definition.".CapitalizeFirst();
			}
		}

		internal void DoEditInterface(PawnFilterEditListing listing) =>
			this.DoEditInterface(listing, out _);

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
			this.def is null;

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
