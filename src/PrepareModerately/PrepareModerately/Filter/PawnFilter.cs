using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using Lakuna.PrepareModerately.Filter.Part;
using Lakuna.PrepareModerately.Utility;

using RimWorld;

using Verse;

namespace Lakuna.PrepareModerately.Filter {
	// Based on `RimWorld.Scenario`.
	public sealed class PawnFilter : IExposable {
		public static IEnumerable<Version> SupportedVersions =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Version[] { new Version(VersionControl.CurrentMajor, VersionControl.CurrentMinor) };
#else
			[new(VersionControl.CurrentMajor, VersionControl.CurrentMinor)];
#endif

		internal static PawnFilter Current {
			get; set;
		}

		internal static readonly string s_dataFolder = $"PawnFilters{Path.DirectorySeparatorChar}";

		internal const int DescriptionMaxLength = 1000;

		internal const string FileExtension = ".rpf";

		internal const int NameMaxLength = 55;

		internal const int SummaryMaxLength = 300;

		// Based on `Verse.GenFilePaths.AllCustomScenarioFiles`.
		internal static IList<FileInfo> AllFiles {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				DirectoryInfo directoryInfo = new DirectoryInfo(DataPath);
#else
				DirectoryInfo directoryInfo = new(DataPath);
#endif
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				return directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime).ToList();
#else
				return [.. directoryInfo
					.GetFiles()
					.Where((file) => file.Extension == FileExtension)
					.OrderByDescending((file) => file.LastWriteTime)];
#endif
			}
		}

		internal static string DefaultDataPath =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, new object[] { s_dataFolder });
#else
			(string)typeof(GenFilePaths)
				.GetMethod("FolderUnderSaveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
				.Invoke(null, [s_dataFolder]);
#endif

		internal static IEnumerable<PawnFilter> LocalFilters =>
			s_localFiltersInternal;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly Collection<PawnFilter> s_localFiltersInternal = new Collection<PawnFilter>();
#else
		private static readonly Collection<PawnFilter> s_localFiltersInternal = [];
#endif

		private static string DataPath =>
			PrepareModeratelyMod.Settings.FilterSavePath.NullOrEmpty() ? DefaultDataPath
			: PrepareModeratelyMod.Settings.FilterSavePath;

		public PawnFilterCategory Category {
			get {
				if (this._category == PawnFilterCategory.Undefined) {
					PrepareModeratelyLogger.LogErrorMessage("Filter category is undefined.");
				}

				return this._category;
			}

			set => this._category = value;
		}

		public string Description {
			get => this._description;
			set => this._description = value;
		}

		public string Name {
			get => this._name;
			set => this._name = value;
		}

		public IEnumerable<PawnFilterPart> Parts {
			get => this._parts;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			internal set => this._parts = value.ToList();
#else
			internal set => this._parts = [.. value];
#endif
		}

		public string Summary {
			get => this._summary;
			set => this._summary = value;
		}

		internal bool Valid { get; set; } = true;

		internal PawnFilter CopyForEditing {
			get {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				PawnFilter copyForEditing = new PawnFilter {
					Name = this.Name,
					Summary = this.Summary,
					Description = this.Description,
					Category = PawnFilterCategory.CustomLocal
				};
#else
				PawnFilter copyForEditing = new() {
					Name = this.Name,
					Summary = this.Summary,
					Description = this.Description,
					Category = PawnFilterCategory.CustomLocal
				};
#endif
				copyForEditing._parts.AddRange(this.Parts.Select((part) => part.CopyForEditing));
				return copyForEditing;
			}
		}

		internal bool Enabled { get; } = true;

		internal FileInfo File =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new FileInfo(AbsolutePathForName(this.FileName));
#else
			new(AbsolutePathForName(this.FileName));
#endif

		[field: NoTranslate]
		internal string FileName {
			get; set;
		}

		internal string FullInformationText {
			get {
				try {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					StringBuilder stringBuilder = new StringBuilder();
#else
					StringBuilder stringBuilder = new();
#endif
					_ = stringBuilder.AppendLine(this.Description);
					_ = stringBuilder.AppendLine();

					foreach (PawnFilterPart part in this.Parts) {
						part.Summarized = false;
					}

					foreach (PawnFilterPart part in
						from part in this.Parts
						orderby part.Def.SummaryPriority descending, part.Def.defName
						where part.Visible
						select part) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
						string summary = $"{part.Summary(this).CapitalizeFirst()}.";
#else
						string summary = part.Summary(this).CapitalizeFirst().EndWithPeriod();
#endif
						if (!summary.NullOrEmpty()) {
							_ = stringBuilder.AppendLine(summary);
						}
					}

					return stringBuilder.ToString().TrimEndNewlines();
#pragma warning disable CA1031 // Don't rethrow the exception to avoid messing with the game.
				} catch (Exception e) {
#pragma warning restore CA1031
					PrepareModeratelyLogger.LogException(e, "Failed to get full information text.", PrepareModeratelyLoggerCategory.GetFullInformationText);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
					return $"{"PM.FailedToGetFullInformationText".Translate().CapitalizeFirst()}.";
#else
					return "PM.FailedToGetFullInformationText".Translate().CapitalizeFirst().EndWithPeriod();
#endif
				}
			}
		}

		internal bool ShowInUi { get; } = true;

		private PawnFilterCategory _category;

		[MustTranslate]
		private string _description;

		[MustTranslate]
		private string _name;

#pragma warning disable CA1002 // RimWorld requires a `List` to serialize with `Scribe_Collections.Look`.
		private List<PawnFilterPart> _parts;
#pragma warning restore CA1002

		[MustTranslate]
		private string _summary;

		public PawnFilter() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			this._parts = new List<PawnFilterPart>();
#else
			this._parts = [];
#endif

		// Based on `Verse.GenFilePaths.AbsPathForScenario`.
		internal static string AbsolutePathForName(string filterName) =>
			Path.Combine(DataPath, filterName + FileExtension);

		// Based on `RimWorld.ScenarioFiles.RecacheData`.
		internal static void RecacheLocalFilters() {
			s_localFiltersInternal.Clear();
			foreach (FileInfo file in AllFiles) {
				if (PawnFilterSaveLoader.Load(file.FullName, PawnFilterCategory.CustomLocal, out PawnFilter filter)) {
					s_localFiltersInternal.Add(filter);
				}
			}
		}

		public void AddPart(PawnFilterPart part, bool prepend = false) {
			if (prepend) {
				this._parts.Insert(0, part);
				return;
			}

			this._parts.Add(part);
		}

		public void ExposeData() {
			Scribe_Values.Look(ref this._name, MiscUtil.NoUnder(nameof(this._name)));
			Scribe_Values.Look(ref this._summary, MiscUtil.NoUnder(nameof(this._summary)));
			Scribe_Values.Look(ref this._description, MiscUtil.NoUnder(nameof(this._description)));
			Scribe_Collections.Look(ref this._parts, MiscUtil.NoUnder(nameof(this._parts)), LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (this._parts.RemoveAll((part) => !part.IsValid()) > 0) {
					PrepareModeratelyLogger.LogErrorMessage("Some filter parts were invalid.");
				}
			}
		}

		public override int GetHashCode() {
			int hash = 6126121;
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

			int i = 0;
			foreach (PawnFilterPart part in this.Parts) {
				hash ^= part.GetHashCode() + i++;
			}

			return hash;
		}

		public bool Matches(Pawn pawn) {
			foreach (PawnFilterPart part in this.Parts) {
				if (!part.Matches(pawn)) {
					return false;
				}
			}

			return true;
		}

		public void RemovePart(PawnFilterPart part) {
			if (!this.Parts.Contains(part)) {
				PrepareModeratelyLogger.LogErrorMessage("Failed to remove filter part.");
			}

			_ = this._parts.Remove(part);
		}

		public override string ToString() =>
			this.Name.NullOrEmpty() ? "PM.UnnamedFilter".Translate().ToString().CapitalizeFirst()
			: this.Name;

		internal bool CanReorder(PawnFilterPart part, ReorderDirection dir) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (part is null) {
				throw new ArgumentNullException(nameof(part));
			}
#else
			ArgumentNullException.ThrowIfNull(part);
#endif

			if (!part.Def.PlayerAddRemovable) {
				return false;
			}

			int index = this._parts.IndexOf(part);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			switch (dir) {
				case ReorderDirection.Up:
					return index > 0 && this._parts[index - 1].Def.PlayerAddRemovable;
				case ReorderDirection.Down:
					return index != this._parts.Count - 1;
				default:
					throw new NotImplementedException();
			}
#else
			return dir switch {
				ReorderDirection.Up => index > 0 && this._parts[index - 1].Def.PlayerAddRemovable,
				ReorderDirection.Down => index != this._parts.Count - 1,
				_ => throw new NotImplementedException(),
			};
#endif
		}

		internal IEnumerable<string> ConfigErrors() {
			if (this.Name.NullOrEmpty()) {
				yield return "No title.";
			}

			// if (this.parts.NullOrEmpty()) { yield return "No parts."; }

			foreach (PawnFilterPart part in this.Parts) {
				if (part is null) {
					yield return "Filter has null part.";
					continue;
				}

				foreach (string item in part.ConfigErrors()) {
					yield return item;
				}
			}
		}

		internal void PreConfigure() {
			foreach (PawnFilterPart part in this.Parts) {
				part.PreConfigure();
			}
		}

		internal void Reorder(PawnFilterPart part, ReorderDirection dir) {
			int index = this._parts.IndexOf(part);
			this._parts.RemoveAt(index);
			switch (dir) {
				case ReorderDirection.Up:
					this._parts.Insert(index - 1, part);
					break;
				case ReorderDirection.Down:
					this._parts.Insert(index + 1, part);
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
