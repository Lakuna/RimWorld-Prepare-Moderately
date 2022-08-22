using System.Collections.Generic;
using Lakuna.PrepareModerately.GUI;
using Verse;

namespace Lakuna.PrepareModerately.Filter.FilterPart {
	public abstract class FilterPart : IExposable {
		[TranslationHandle]
		public FilterPartDef def;

		public bool visible;

		public bool summarized;

		public static float RowHeight => Text.LineHeight;

		public virtual string Label => this.def.LabelCap;

		public virtual void ExposeData() {
			Scribe_Defs.Look(ref this.def, nameof(this.def));
		}

		public FilterPart CopyForEditing() {
			FilterPart filterPart = this.CopyForEditingInner();
			filterPart.def = this.def;
			return filterPart;
		}

		protected virtual FilterPart CopyForEditingInner() {
			return (FilterPart)this.MemberwiseClone();
		}

		public virtual void DoEditInterface(FilterEditListing listing) {
			listing.GetFilterPartRect(this, FilterPart.RowHeight);
		}

		public virtual string Summary(Filter filter) {
			return this.def.description;
		}

		public virtual IEnumerable<string> GetSummaryListEntries(string tag) {
			yield break;
		}

		public virtual void Randomize() { }

		public virtual bool TryMerge(FilterPart other) {
			return false;
		}

		public virtual bool CanCoexistWith(FilterPart other) {
			return true;
		}

		public virtual bool AllowPlayerStartingPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req) {
			return true;
		}

		public virtual IEnumerable<string> ConfigErrors() {
			if (this.def == null) {
				yield return this.GetType().ToString() + " has null def.";
			}
		}

		public virtual bool HasNullDefs() {
			return this.def == null;
		}

		public FilterPart() {
			this.visible = true;
		}
	}
}
