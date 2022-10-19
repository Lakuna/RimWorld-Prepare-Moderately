using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using Verse;
using System.Linq;
using System.Text;

namespace Lakuna.PrepareModerately.Filter {
	public class PawnFilter : IExposable {
		public enum PawnFilterCategory {
			Undefined,
			FromDef,
			CustomLocal
		}

		public static PawnFilter Current { get; set; }

		[MustTranslate]
		private string name;

		public string Name => this.name;

		[MustTranslate]
		private string summary;

		public string Summary => this.summary;

		[MustTranslate]
		private string description;

		public string Description => this.description;

		private List<PawnFilterPart> parts;

		private PawnFilterCategory category;

		[NoTranslate]
		private string fileName;

		public string FileName => this.fileName;

		public bool Enabled { get; set; }

		public bool ShowInUi { get; set; }

		public const int NameMaxLength = 55;

		public const int SummaryMaxLength = 300;

		public const int DescriptionMaxLength = 1000;

		public static IEnumerable<Version> SupportedVersions {
			get {
				yield return new Version(VersionControl.CurrentMajor, VersionControl.CurrentMinor);
			}
		}

		public FileInfo File => new FileInfo(AbsolutePathForPawnFilter(this.FileName));

		public IEnumerable<PawnFilterPart> AllParts {
			get {
				for (int i = 0; i < this.parts.Count; i++) {
					yield return this.parts[i];
				}
			}
		}

		public PawnFilterCategory Category {
			get {
				if (this.category == PawnFilterCategory.Undefined) {
					Logger.LogErrorMessage("Filter category is undefined.");
				}

				return this.category;
			}

			set => this.category = value;
		}

		public void ExposeData() {
			Scribe_Values.Look(ref this.name, nameof(this.name));
			Scribe_Values.Look(ref this.summary, nameof(this.summary));
			Scribe_Values.Look(ref this.description, nameof(this.description));
			Scribe_Collections.Look(ref this.parts, nameof(this.parts), LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this.parts.RemoveAll((PawnFilterPart part) => part == null) != 0) {
					Logger.LogErrorMessage("Some filter parts were null after loading.");
				}

				if (this.parts.RemoveAll((PawnFilterPart part) => part.HasNullDefs()) != 0) {
					Logger.LogErrorMessage("Some filter parts had null definitions.");
				}
			}
		}

		public IEnumerable<string> ConfigErrors() {
			if (this.name.NullOrEmpty()) { yield return "No title."; }
			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (PawnFilterPart part in this.AllParts) {
				foreach (string item in part.ConfigErrors()) {
					yield return item;
				}
			}
		}

		public string FullInformationText {
			get {
				try {
					StringBuilder stringBuilder = new StringBuilder();
					_ = stringBuilder.AppendLine(this.description);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.AllParts) {
						part.Summarized = false;
					}

					foreach (PawnFilterPart part in from part in this.AllParts orderby part.Def.SummaryPriority descending, part.Def.defName where part.Visible select part) {
						string summary = part.Summary(this).CapitalizeFirst() + ".";
						if (!summary.NullOrEmpty()) { _ = stringBuilder.AppendLine(summary); }
					}

					return stringBuilder.ToString().TrimEndNewlines();
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					Logger.LogException(e, "Failed to get full information text.", Logger.LoggerCategory.GetFullInformationText);
					return "FailedToGetFullInformationText".Translate().CapitalizeFirst() + ".";
				}
			}
		}

		public PawnFilter CopyForEditing() {
			PawnFilter copyForEditing = new PawnFilter {
				name = this.Name,
				summary = this.Summary,
				description = this.Description,
				category = PawnFilterCategory.CustomLocal
			};
			copyForEditing.parts.AddRange(this.parts.Select((PawnFilterPart part) => part.CopyForEditing()));
			return copyForEditing;
		}

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.AllParts) {
				if (!part.Matches(pawn)) { return false; }
			}
			return true;
		}

		public void RemovePart(PawnFilterPart part) {
			if (!this.parts.Contains(part)) { Logger.LogErrorMessage("Failed to remove filter part."); }
			_ = this.parts.Remove(part);
		}

		public bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
			if (part == null) {
				throw new ArgumentNullException(nameof(part));
			}

			if (!part.Def.PlayerAddRemovable) { return false; }

			int index = this.parts.IndexOf(part);
			switch (dir) {
				case ReorderDirection.Up:
					if (index == 0) { return false; }
					if (index > 0 && !this.parts[index - 1].Def.PlayerAddRemovable) { return false; }
					return true;
				case ReorderDirection.Down:
					return index != this.parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
		}

		public void Reorder(PawnFilterPart part, ReorderDirection dir) {
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

		public override string ToString() => this.name.NullOrEmpty() ? "UnnamedFilter".Translate().ToString().CapitalizeFirst() : this.name;

		public override int GetHashCode() {
			int hash = 5251977;
			if (this.Name != null) { hash ^= this.Name.GetHashCode(); }
			if (this.Summary != null) { hash ^= this.Summary.GetHashCode(); }
			if (this.Description != null) { hash ^= this.Description.GetHashCode(); }
			return hash;
		}

		public PawnFilter() {
			this.parts = new List<PawnFilterPart>();
			this.Enabled = true;
			this.ShowInUi = true;
		}

		public const string PawnFilterExtension = ".rpf";

		public const string PawnFiltersFolder = "PawnFilters/";

		public static string DefaultPawnFiltersFolderPath => (string)typeof(GenFilePaths).GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, new object[] { PawnFiltersFolder });

		public static string PawnFiltersFolderPath => PrepareModeratelyMod.Settings.FilterSavePath.NullOrEmpty() ? DefaultPawnFiltersFolderPath : PrepareModeratelyMod.Settings.FilterSavePath;

		public static IEnumerable<FileInfo> AllCustomFilterFiles {
			get {
				DirectoryInfo directoryInfo = new DirectoryInfo(PawnFiltersFolderPath);
				if (!directoryInfo.Exists) { directoryInfo.Create(); }
				return from file in directoryInfo.GetFiles() where file.Extension == PawnFilterExtension orderby file.LastWriteTime descending select file;
			}
		}

		public static string AbsolutePathForPawnFilter(string filterName) => Path.Combine(PawnFiltersFolderPath, filterName + PawnFilterExtension);
	}
}
