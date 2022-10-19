using System;
using UnityEngine;
using Verse;

[assembly: CLSCompliant(false)]
namespace Lakuna.PrepareModerately {
	public class PrepareModeratelyMod : Mod {
		private static PrepareModeratelySettings settings;

		public static PrepareModeratelySettings Settings => PrepareModeratelyMod.settings;

		public PrepareModeratelyMod(ModContentPack content) : base(content) => PrepareModeratelyMod.settings = this.GetSettings<PrepareModeratelySettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.Label("FilterSavePath".Translate().CapitalizeFirst());
			PrepareModeratelyMod.Settings.FilterSavePath = listing.TextEntry(PrepareModeratelyMod.settings.FilterSavePath);

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() => "PrepareModerately".Translate().CapitalizeFirst();
	}
}
