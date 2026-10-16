using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately {
#pragma warning disable CA1812 // Class is instantiated via RimWorld.
	internal sealed class PrepareModeratelyMod : Mod {
#pragma warning restore CA1812
		internal static PrepareModeratelySettings _settings {
			get; private set;
		}

		// Must be public.
		public PrepareModeratelyMod(ModContentPack content) : base(content) =>
			_settings = this.GetSettings<PrepareModeratelySettings>();

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
			_settings.filterSavePath = Widgets.TextArea(filterSavePathRect.RightPart(1 - labelWidthPercentage), _settings.filterSavePath);

			listing.Gap(); // So that the horizontal slider's bound numbers don't intersect with the text area.

			Rect rollSpeedMultiplierRect = listing.GetRect(Text.LineHeight);
			Widgets.Label(rollSpeedMultiplierRect.LeftPart(labelWidthPercentage), "PM.RollSpeedMultiplierNumber".Translate(_settings.rollSpeedMultiplier).CapitalizeFirst());
#if V1_0 || V1_1 || V1_2 || V1_3
			_settings.rollSpeedMultiplier = (int)Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), _settings.rollSpeedMultiplier, 1, 100);
#elif V1_4 || V1_5 || V1_6
			Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), ref _settings.rollSpeedMultiplier, new FloatRange(1, 100));
#else
			Widgets.HorizontalSlider(rollSpeedMultiplierRect.RightPart(1 - labelWidthPercentage), ref _settings.rollSpeedMultiplier, new(1, 100));
#endif

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() =>
			"PM.PrepareModerately".Translate().CapitalizeFirst();
	}
}
