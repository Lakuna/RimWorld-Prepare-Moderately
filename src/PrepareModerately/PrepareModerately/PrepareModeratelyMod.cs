using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately {
	public sealed class PrepareModeratelyMod : Mod {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static PrepareModeratelySettings Settings
#else
		public static PrepareModeratelySettings? Settings
#endif
		{
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
			Widgets.Label(filterSavePathRect.LeftPart(_labelWidthPercentage).Rounded(), "PM.FilterSavePath".TranslateFull(CapitalizeType.First));
			Settings.FilterSavePath = Widgets.TextArea(filterSavePathRect.RightPart(1 - _labelWidthPercentage).Rounded(), Settings.FilterSavePath);
			if (Settings.FilterSavePath.NullOrEmpty()) {
				Widgets.Label(listing.GetRect(Text.LineHeight).Rounded(), $"{"PM.UsingDefaultValue".TranslateFull(CapitalizeType.First)}: {PawnFilterFilePaths.DefaultDataPath}");
			}

			listing.Gap(); // So that the horizontal slider's bound numbers don't intersect with the text area.

			float rollSpeedMultiplier = Settings.RollSpeedMultiplier;
			string rollSpeedLabel = "PM.RollSpeedMultiplierNumber".TranslateFull(CapitalizeType.First, Settings.RollSpeedMultiplier);
			Rect rollSpeedRect = listing.GetRect(Text.LineHeight).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			FloatRange rollSpeedRange = new FloatRange(1, 100);
#else
			FloatRange rollSpeedRange = new(1, 100);
#endif
			float rollSpeedIncrement = 1;
#if V1_0 || V1_1 || V1_2 || V1_3
			rollSpeedMultiplier = Widgets.HorizontalSlider(rollSpeedRect, rollSpeedMultiplier, rollSpeedRange.min, rollSpeedRange.max, false, rollSpeedLabel, null, null, rollSpeedIncrement);
#else
			Widgets.HorizontalSlider(rollSpeedRect, ref rollSpeedMultiplier, rollSpeedRange, rollSpeedLabel, rollSpeedIncrement);
#endif
			Settings.RollSpeedMultiplier = rollSpeedMultiplier;

			bool highVisibilityMode = Settings.HighVisibilityMode;
			Widgets.CheckboxLabeled(listing.GetRect(Text.LineHeight).Rounded(), "PM.HighVisibilityMode".TranslateFull(CapitalizeType.First), ref highVisibilityMode);
			Settings.HighVisibilityMode = highVisibilityMode;

			listing.End();
		}

		public override string SettingsCategory() =>
			"PM.PrepareModerately".TranslateFull(CapitalizeType.Title);
	}
}
