﻿using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.Scenario`.
	public class PawnFilter : IExposable {
		public static PawnFilter Current { get; set; }

		[MustTranslate]
		private string name;

		public string Name {
			get => this.name;
			set => this.name = value;
		}

		[MustTranslate]
		private string summary;

		public string Summary {
			get => this.summary;
			set => this.summary = value;
		}

		[MustTranslate]
		private string description;

		public string Description {
			get => this.description;
			set => this.description = value;
		}

		private List<PawnFilterPart> parts;

		public IEnumerable<PawnFilterPart> Parts => this.parts;

		public void AddPart(PawnFilterPart part, bool prepend = false) {
			if (prepend) {
				this.parts.Insert(0, part);
			} else {
				this.parts.Add(part);
			}
		}

		public void RemovePart(PawnFilterPart part) {
			if (!this.parts.Contains(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}

			_ = this.parts.Remove(part);
		}

		private PawnFilterCategory category;

		public PawnFilterCategory Category {
			get {
				if (this.category == PawnFilterCategory.Undefined) {
					PrepareModeratelyLogger.LogErrorMessage("Filter category is undefined.");
				}

				return this.category;
			}

			set => this.category = value;
		}

		[field: NoTranslate]
		public string FileName { get; set; }

		public bool Enabled { get; }

		public bool ShowInUi { get; }

		public const int NameMaxLength = 55;

		public const int SummaryMaxLength = 300;

		public const int DescriptionMaxLength = 1000;

		public static IEnumerable<Version> SupportedVersions {
			get {
				yield return new Version(VersionControl.CurrentMajor, VersionControl.CurrentMinor);
			}
		}

		public FileInfo File => new FileInfo(AbsolutePathForName(this.FileName));

		public void ExposeData() {
			Scribe_Values.Look(ref this.name, nameof(this.name));
			Scribe_Values.Look(ref this.summary, nameof(this.summary));
			Scribe_Values.Look(ref this.description, nameof(this.description));
			Scribe_Collections.Look(ref this.parts, nameof(this.parts), LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this.parts.RemoveAll((part) => part == null) != 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts were null after loading.");
				}

				if (this.parts.RemoveAll((part) => part.HasNullDefs()) != 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts had null definitions.");
				}
			}
		}

		public IEnumerable<string> ConfigErrors() {
			if (this.Name.NullOrEmpty()) {
				yield return "No title.";
			}

			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (PawnFilterPart part in this.Parts) {
				foreach (string item in part.ConfigErrors()) {
					yield return item;
				}
			}
		}

		public string FullInformationText {
			get {
				try {
					StringBuilder stringBuilder = new StringBuilder();
					_ = stringBuilder.AppendLine(this.Description);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.Parts) {
						part.Summarized = false;
					}

					foreach (PawnFilterPart part in
						from part in this.Parts
						orderby part.Def.summaryPriority descending,
						part.Def.defName
						where part.Visible
						select part) {
						string summary = part.Summary(this).CapitalizeFirst() + ".";
						if (!summary.NullOrEmpty()) {
							_ = stringBuilder.AppendLine(summary);
						}
					}

					return stringBuilder.ToString().TrimEndNewlines();
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					PrepareModeratelyLogger.LogException(e, "Failed to get full information text.", PrepareModeratelyLoggerCategory.GetFullInformationText);
					return "FailedToGetFullInformationText".Translate().CapitalizeFirst() + ".";
				}
			}
		}

		public PawnFilter CopyForEditing {
			get {
				PawnFilter copyForEditing = new PawnFilter {
					Name = this.Name,
					Summary = this.Summary,
					Description = this.Description,
					Category = PawnFilterCategory.CustomLocal
				};
				copyForEditing.parts.AddRange(this.parts.Select((part) => part.CopyForEditing()));
				return copyForEditing;
			}
		}

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.Parts) {
				if (!part.Matches(pawn)) {
					return false;
				}
			}

			return true;
		}

		public bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
			if (part == null) {
				throw new ArgumentNullException(nameof(part));
			}

			if (!part.Def.PlayerAddRemovable) {
				return false;
			}

			int index = this.parts.IndexOf(part);
			switch (dir) {
				case ReorderDirection.Up:
					if (index == 0) {
						return false;
					}

					if (index > 0 && !this.parts[index - 1].Def.PlayerAddRemovable) {
						return false;
					}

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

		public override string ToString() => this.Name.NullOrEmpty() ? "UnnamedFilter".Translate().ToString().CapitalizeFirst() : this.Name;

		public override int GetHashCode() {
			int hash = 5251977;
			if (this.Name != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.Name.GetHashCode();
#else
				hash ^= this.Name.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.Summary != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.Summary.GetHashCode();
#else
				hash ^= this.Summary.GetHashCode(StringComparison.Ordinal);
#endif
			}

			if (this.Description != null) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				hash ^= this.Description.GetHashCode();
#else
				hash ^= this.Description.GetHashCode(StringComparison.Ordinal);
#endif
			}

			return hash;
		}

		public PawnFilter() {
			this.parts = new List<PawnFilterPart>();
			this.Enabled = true;
			this.ShowInUi = true;
		}

		public const string FileExtension = ".rpf";

		public const string DataFolder = "PawnFilters/";

		public static string DefaultDataPath => (string)typeof(GenFilePaths)
			.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
			.Invoke(null, new object[] { DataFolder });

		public static string DataPath => PrepareModeratelyMod.Settings.FilterSavePath.NullOrEmpty() ? DefaultDataPath : PrepareModeratelyMod.Settings.FilterSavePath;

		public static IEnumerable<FileInfo> AllFiles {
			get {
				DirectoryInfo directoryInfo = new DirectoryInfo(DataPath);
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

				return from file in directoryInfo.GetFiles() where file.Extension == FileExtension orderby file.LastWriteTime descending select file;
			}
		}

		public static string AbsolutePathForName(string filterName) => Path.Combine(DataPath, filterName + FileExtension);

		private static readonly List<PawnFilter> LocalFiltersInternal = new List<PawnFilter>();

		public static IEnumerable<PawnFilter> LocalFilters => LocalFiltersInternal;

		public static void RecacheLocalFilters() {
			LocalFiltersInternal.Clear();
			foreach (FileInfo file in AllFiles) {
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					LocalFiltersInternal.Add(filter);
				}
			}
		}
	}
}
