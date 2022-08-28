using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately {
	public class PrepareModerately : Mod {
		public static Settings settings;

		public PrepareModerately(ModContentPack content) : base(content) {
			PrepareModerately.settings = this.GetSettings<Settings>();
		}

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.Label("FilterSavePath".Translate().CapitalizeFirst());
			PrepareModerately.settings.filterSavePath = listing.TextEntry(PrepareModerately.settings.filterSavePath);

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() {
			return "PrepareModerately".Translate().CapitalizeFirst();
		}
	}
}
