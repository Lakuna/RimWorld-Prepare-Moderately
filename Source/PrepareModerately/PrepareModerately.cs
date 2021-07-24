using PrepareModerately.UI;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PrepareModerately : Mod {
		public const string filterExtension = ".v2.xml";

		public static PrepareModerately Instance;
		public static readonly string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Lakuna\\PrepareModerately";

		public static void LogError(Exception e) {
			string output = "Prepare Moderately encountered an exception.";
			Exception innerException = e;
			while (innerException != null) {
				output += "\n>" + innerException.Message;
				innerException = innerException.InnerException;
			}
			output += "\n\nStack trace:\n" + e.StackTrace + "\n\n";
			Log.Error(output);
		}

		public Settings settings;
		public Page_ConfigureStartingPawns configureStartingPawnsPage;
		public PawnFilter.PawnFilter activeFilter;
		public Pawn activePawn;
		public bool activelyRolling;
		private Page_PrepareModerately page;
		public Page_PrepareModerately Page {
			get {
				if (this.page == null) {
					this.page = new Page_PrepareModerately();
				}

				return this.page;
			}
		}

		public PrepareModerately(ModContentPack content) : base(content) {
			if (PrepareModerately.Instance == null) {
				PrepareModerately.Instance = this;
			}

			this.settings = this.GetSettings<Settings>();
			this.activeFilter = new PawnFilter.PawnFilter();
			this.activelyRolling = false;
		}

		public override void DoSettingsWindowContents(Rect rect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(rect);

			listing.Label("Pawns to generate per roll step (" + this.settings.rollMultiplier + ")");
			this.settings.rollMultiplier = (int) listing.Slider(this.settings.rollMultiplier, 1, 10);

			listing.Label("Roll steps between pawn generation (" + this.settings.rollModulus + ")");
			this.settings.rollModulus = (int) listing.Slider(this.settings.rollModulus, 1, 1000);

			listing.End();

			base.DoSettingsWindowContents(rect);
		}

		public override string SettingsCategory() => "Prepare Moderately";
	}
}
