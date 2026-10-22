using System.Collections.Generic;

using Lakuna.PrepareModerately.Ui;
using Lakuna.PrepareModerately.Utility;

using Verse;

namespace Lakuna.PrepareModerately.Filter.Part {
	// Based on `RimWorld.ScenPart`.
	public abstract class PawnFilterPart : IExposable {
		public virtual IEnumerable<string> ConfigErrors {
			get {
				yield break;
			}
		}

		public virtual PawnFilterPart CopyForEditing =>
			(PawnFilterPart)this.MemberwiseClone();

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public PawnFilterPartDef Def {
#else
		public PawnFilterPartDef? Def {
#endif
			get => this._def;
			set => this._def = value;
		}

		public virtual bool HasNullDefs =>
			false;

		public virtual string Label =>
			this.Def?.label ?? "null";

		public virtual bool Valid =>
			!this.HasNullDefs;

		public bool Visible { get; set; } = true;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private PawnFilterPartDef _def;
#else
		private PawnFilterPartDef? _def;
#endif

		public virtual bool CanCoexistWith(PawnFilterPart other) =>
			true;

		public virtual void DoEditInterface(PawnFilterEditListing listing) =>
			this.DoEditInterface(listing, out _);

		public virtual void DoEditInterface(PawnFilterEditListing listing, out float totalAddedListHeight) {
			totalAddedListHeight = 0;
			if (listing is null) {
				PrepareModeratelyLogger.LogErrorMessage("Attempted to draw filter part edit interface on null listing.");
				return;
			}

			_ = listing.GetPawnFilterPartRect(this, 0, out totalAddedListHeight).Rounded();
		}

		public virtual void ExposeData() =>
			Scribe_Defs.Look(ref this._def, "def");

		public override int GetHashCode() =>
			this.Def?.GetHashCode() ?? 0;

		public virtual bool IgnoreFor(Pawn pawn) =>
			!this.Valid;

		public virtual bool Matches(Pawn pawn) =>
			true;

		public virtual void PreConfigure() {
		}

		public virtual void Randomize() {
		}

		public virtual string Summary(PawnFilter filter) =>
			this.Def?.description ?? "";

		public virtual bool TryMerge(PawnFilterPart other) =>
			false;
	}
}
