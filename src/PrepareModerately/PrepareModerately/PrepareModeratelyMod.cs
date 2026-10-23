using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately {
	public sealed class PrepareModeratelyMod : Mod {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static PrepareModeratelySettings Settings {
#else
		public static PrepareModeratelySettings? Settings {
#endif
			get; private set;
		}

		private const float _labelWidthPercentage = 0.2f;

		public PrepareModeratelyMod(ModContentPack content) : base(content) =>
			Settings = this.GetSettings<PrepareModeratelySettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			if (Settings is null) {
				PrepareModeratelyLogger.LogErrorMessage("Tried to draw settings window contents with null settings.");
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard();
#else
			Listing_Standard listing = new();
#endif
			listing.Begin(inRect);

			Rect filterSavePathRect = listing.GetRect(Text.LineHeight).Rounded();
#if V1_0
			Widgets.Label(filterSavePathRect.LeftPart(_labelWidthPercentage).Rounded(), "PM.FilterSavePath".Translate().CapitalizeFirst());
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Widgets.Label(filterSavePathRect.LeftPart(_labelWidthPercentage).Rounded(), "PM.FilterSavePath".Translate().CapitalizeFirst().StringSafe());
#else
			Widgets.Label(filterSavePathRect.LeftPart(_labelWidthPercentage).Rounded(), "PM.FilterSavePath".Translate().CapitalizeFirst().StringSafe);
#endif
			Settings.FilterSavePath = Widgets.TextArea(filterSavePathRect.RightPart(1 - _labelWidthPercentage).Rounded(), Settings.FilterSavePath);
			if (Settings.FilterSavePath.NullOrEmpty()) {
#if V1_0
				Widgets.Label(listing.GetRect(Text.LineHeight).Rounded(), $"{"PM.UsingDefaultValue".Translate().CapitalizeFirst()}: {PawnFilterFilePaths.DefaultDataPath}");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				Widgets.Label(listing.GetRect(Text.LineHeight).Rounded(), $"{"PM.UsingDefaultValue".Translate().CapitalizeFirst().StringSafe()}: {PawnFilterFilePaths.DefaultDataPath}");
#else
				Widgets.Label(listing.GetRect(Text.LineHeight).Rounded(), $"{"PM.UsingDefaultValue".Translate().CapitalizeFirst().StringSafe}: {PawnFilterFilePaths.DefaultDataPath}");
#endif
			}

			listing.Gap(); // So that the horizontal slider's bound numbers don't intersect with the text area.

			float rollSpeedMultiplier = Settings.RollSpeedMultiplier;
#if V1_0
			rollSpeedMultiplier = Widgets.HorizontalSlider(listing.GetRect(Text.LineHeight).Rounded(), rollSpeedMultiplier, 1, 100, false, "PM.RollSpeedMultiplierNumber".Translate(Settings.RollSpeedMultiplier)?.CapitalizeFirstSafe() ?? "null", null, null, 1);
#elif V1_1 || V1_2 || V1_3
			rollSpeedMultiplier = Widgets.HorizontalSlider(listing.GetRect(Text.LineHeight).Rounded(), rollSpeedMultiplier, 1, 100, false, "PM.RollSpeedMultiplierNumber".Translate(Settings.RollSpeedMultiplier).CapitalizeFirst().StringSafe(), null, null, 1);
#elif V1_4 || V1_5 || V1_6
			Widgets.HorizontalSlider(listing.GetRect(Text.LineHeight).Rounded(), ref rollSpeedMultiplier, new FloatRange(1, 100), "PM.RollSpeedMultiplierNumber".Translate(Settings.RollSpeedMultiplier).CapitalizeFirst().StringSafe(), 1);
#else
			Widgets.HorizontalSlider(listing.GetRect(Text.LineHeight).Rounded(), ref rollSpeedMultiplier, new(1, 100), "PM.RollSpeedMultiplierNumber".Translate(Settings.RollSpeedMultiplier).CapitalizeFirst().StringSafe, 1);
#endif
			Settings.RollSpeedMultiplier = rollSpeedMultiplier;

			bool highVisibilityMode = Settings.HighVisibilityMode;
#if V1_0
			Widgets.CheckboxLabeled(listing.GetRect(Text.LineHeight).Rounded(), "PM.HighVisibilityMode".Translate()?.CapitalizeFirstSafe() ?? "null", ref highVisibilityMode);
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Widgets.CheckboxLabeled(listing.GetRect(Text.LineHeight).Rounded(), "PM.HighVisibilityMode".Translate().CapitalizeFirst().StringSafe(), ref highVisibilityMode);
#else
			Widgets.CheckboxLabeled(listing.GetRect(Text.LineHeight).Rounded(), "PM.HighVisibilityMode".Translate().CapitalizeFirst().StringSafe, ref highVisibilityMode);
#endif
			Settings.HighVisibilityMode = highVisibilityMode;

			listing.End();
		}

		public override string SettingsCategory() =>
#if V1_0
			"PM.PrepareModerately".Translate().CapitalizeAsTitleSafe();
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"PM.PrepareModerately".Translate().StringSafe().CapitalizeAsTitleSafe();
#else
			"PM.PrepareModerately".Translate().StringSafe.CapitalizeAsTitleSafe;
#endif
	}
}
