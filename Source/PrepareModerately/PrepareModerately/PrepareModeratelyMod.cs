using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately {
	public class PrepareModeratelyMod : Mod {
		public static PrepareModeratelySettings Settings { get; private set; }

		public PrepareModeratelyMod(ModContentPack content) : base(content) => Settings = this.GetSettings<PrepareModeratelySettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			float labelWidthPercentage = 0.2f;

			Rect filterSavePathRect = listing.GetRect(Text.LineHeight);
			Widgets.Label(filterSavePathRect.LeftPart(labelWidthPercentage), "FilterSavePath".Translate().CapitalizeFirst());
			Settings.FilterSavePath = Widgets.TextArea(filterSavePathRect.RightPart(1 - labelWidthPercentage), Settings.FilterSavePath);

			listing.Gap(); // So that the horizontal slider's bound numbers don't intersect with the text area.

			Rect rollSpeedMultiplierRect = listing.GetRect(Text.LineHeight);
			Widgets.Label(rollSpeedMultiplierRect.LeftPart(labelWidthPercentage), "RollSpeedMultiplierNumber".Translate(Settings.RollSpeedMultiplier).CapitalizeFirst());
#if V1_0 || V1_1 || V1_2 || V1_3
			Settings.RollSpeedMultiplier = (int)Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), Settings.RollSpeedMultiplier, 1, 100);
#else
			float rollSpeedMultiplier = Settings.RollSpeedMultiplier;
			Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), ref rollSpeedMultiplier, new FloatRange(1, 100));
			Settings.RollSpeedMultiplier = (int)rollSpeedMultiplier;
#endif

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() => "PrepareModerately".Translate().CapitalizeFirst();
	}
}
