using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately {
#pragma warning disable CA1812 // Class is instantiated via RimWorld.
	internal sealed class PrepareModeratelyMod : Mod {
#pragma warning restore CA1812
		internal static PrepareModeratelySettings Settings {
			get; private set;
		}

		// Must be public.
		public PrepareModeratelyMod(ModContentPack content) : base(content) =>
			Settings = this.GetSettings<PrepareModeratelySettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard();
#else
			Listing_Standard listing = new();
#endif

			listing.Begin(inRect);

			float labelWidthPercentage = 0.2f;

			Rect filterSavePathRect = listing.GetRect(Text.LineHeight);
			Widgets.Label(filterSavePathRect.LeftPart(labelWidthPercentage), "PM.FilterSavePath".Translate().CapitalizeFirst());
			Settings.FilterSavePath = Widgets.TextArea(filterSavePathRect.RightPart(1 - labelWidthPercentage), Settings.FilterSavePath);

			listing.Gap(); // So that the horizontal slider's bound numbers don't intersect with the text area.

			Rect rollSpeedMultiplierRect = listing.GetRect(Text.LineHeight);
			Widgets.Label(rollSpeedMultiplierRect.LeftPart(labelWidthPercentage), "PM.RollSpeedMultiplierNumber".Translate(Settings.RollSpeedMultiplier).CapitalizeFirst());

#if V1_0 || V1_1 || V1_2 || V1_3
			Settings.RollSpeedMultiplier = (int)Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), Settings.RollSpeedMultiplier, 1, 100);
#elif V1_4 || V1_5 || V1_6
			float rollSpeedMultiplier = Settings.RollSpeedMultiplier;
			Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), ref rollSpeedMultiplier, new FloatRange(1, 100));
			Settings.RollSpeedMultiplier = (int)rollSpeedMultiplier;
#else
			float rollSpeedMultiplier = Settings.RollSpeedMultiplier;
			Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), ref rollSpeedMultiplier, new(1, 100));
			Settings.RollSpeedMultiplier = (int)rollSpeedMultiplier;
#endif

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() =>
			"PM.PrepareModerately".Translate().CapitalizeFirst();
	}
}
