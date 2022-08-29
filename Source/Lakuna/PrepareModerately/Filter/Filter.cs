using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	public class Filter : IExposable {
		public static Filter currentFilter;

		[MustTranslate]
		public string name;

		[MustTranslate]
		public string summary;

		[MustTranslate]
		public string description;

		internal List<FilterPart.FilterPart> parts;

		private FilterCategory category;

		[NoTranslate]
		public string fileName;

		public bool enabled;

		public bool showInUI;

		public const int NameMaxLength = 55;

		public const int SummaryMaxLength = 300;

		public const int DescriptionMaxLength = 1000;

		public IEnumerable<System.Version> SupportedVersions {
			get {
				yield return new System.Version(VersionControl.CurrentMajor, VersionControl.CurrentMinor);
			}
		}

		public FileInfo File => new FileInfo(GenFilterPaths.AbsolutePathForFilter(this.fileName));

		public IEnumerable<FilterPart.FilterPart> AllParts {
			get {
				for (int i = 0; i < this.parts.Count; i++) {
					yield return this.parts[i];
				}
			}
		}

		public FilterCategory Category {
			get {
				if (this.category == FilterCategory.Undefined) {
					Logger.LogErrorMessage("Filter category is undefined.");
				}

				return this.category;
			}

			set {
				this.category = value;
			}
		}

		public void ExposeData() {
			Scribe_Values.Look(ref this.name, nameof(this.name));
			Scribe_Values.Look(ref this.summary, nameof(this.summary));
			Scribe_Values.Look(ref this.description, nameof(this.description));
			Scribe_Collections.Look(ref this.parts, nameof(this.parts), LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this.parts.RemoveAll((FilterPart.FilterPart part) => part == null) != 0) {
					Logger.LogErrorMessage("Some filter parts were null after loading.");
				}

				if (this.parts.RemoveAll((FilterPart.FilterPart part) => part.HasNullDefs()) != 0) {
					Logger.LogErrorMessage("Some filter parts had null definitions.");
				}
			}
		}

		public IEnumerable<string> ConfigErrors() {
			if (this.name.NullOrEmpty()) { yield return "No title."; }
			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (FilterPart.FilterPart part in this.AllParts) {
				foreach (string item in part.ConfigErrors()) {
					yield return item;
				}
			}
		}

		public string GetFullInformationText() {
			try {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.description);
				stringBuilder.AppendLine();

				foreach (FilterPart.FilterPart part in this.AllParts) {
					part.summarized = false;
				}

				foreach (FilterPart.FilterPart part in from part in this.AllParts orderby part.def.summaryPriority descending, part.def.defName where part.visible select part) {
					string summary = part.Summary(this).CapitalizeFirst() + ".";
					if (!summary.NullOrEmpty()) { stringBuilder.AppendLine(summary); }
				}

				return stringBuilder.ToString().TrimEndNewlines();
			} catch (Exception e) {
				Logger.LogException(e, "Failed to get full information text.", LoggerCategory.GetFullInformationText);
				return "FailedToGetFullInformationText".Translate().CapitalizeFirst() + ".";
			}
		}

		public string GetSummary() {
			return this.summary;
		}

		public Filter CopyForEditing() {
			Filter filter = new Filter();
			filter.name = this.name;
			filter.summary = this.summary;
			filter.description = this.description;
			filter.parts.AddRange(this.parts.Select((FilterPart.FilterPart part) => part.CopyForEditing()));
			filter.category = FilterCategory.CustomLocal;
			return filter;
		}

		public bool Matches(Pawn pawn) {
			foreach (FilterPart.FilterPart part in this.AllParts) {
				if (!part.Matches(pawn)) { return false; }
			}

			return true;
		}

		public void RemovePart(FilterPart.FilterPart part) {
			if (!this.parts.Contains(part)) { Logger.LogErrorMessage("Failed to remove filter part."); }
			this.parts.Remove(part);
		}

		public bool CanReorder(FilterPart.FilterPart part, ReorderDirection dir) {
			if (!part.def.PlayerAddRemovable) { return false; }

			int index = this.parts.IndexOf(part);
			switch (dir) {
				case ReorderDirection.Up:
					if (index == 0) { return false; }
					if (index > 0 && !this.parts[index - 1].def.PlayerAddRemovable) { return false; }
					return true;
				case ReorderDirection.Down:
					return index != this.parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
		}

		public void Reorder(FilterPart.FilterPart part, ReorderDirection dir) {
			int index = this.parts.IndexOf(part);
			this.parts.RemoveAt(index);
			switch (dir) {
				case ReorderDirection.Up:
					this.parts.Insert(index - 1, part);
					break;
				case ReorderDirection.Down:
					this.parts.Insert(index + 1, part);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public override string ToString() {
			return this.name.NullOrEmpty() ? "UnnamedFilter".Translate().ToString().CapitalizeFirst() : this.name;
		}

		public override int GetHashCode() {
			int hash = 5251977;
			if (this.name != null) { hash ^= this.name.GetHashCode(); }
			if (this.summary != null) { hash ^= this.summary.GetHashCode(); }
			if (this.description != null) { hash ^= this.description.GetHashCode(); }
			return hash;
		}

		public Filter() {
			this.parts = new List<FilterPart.FilterPart>();
			this.enabled = true;
			this.showInUI = true;
		}
	}
}