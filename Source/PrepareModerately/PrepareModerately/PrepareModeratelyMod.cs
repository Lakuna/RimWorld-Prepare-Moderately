using System;
using UnityEngine;
using Verse;

[assembly: CLSCompliant(false)]
namespace Lakuna.PrepareModerately {
	public class PrepareModeratelyMod : Mod {
		public static PrepareModeratelySettings Settings { get; private set; }

		public PrepareModeratelyMod(ModContentPack content) : base(content) => Settings = this.GetSettings<PrepareModeratelySettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.Label("FilterSavePath".Translate().CapitalizeFirst());
			Settings.FilterSavePath = listing.TextEntry(Settings.FilterSavePath);

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() => "PrepareModerately".Translate().CapitalizeFirst();
	}
}
