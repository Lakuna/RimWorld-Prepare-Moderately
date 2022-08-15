using System;
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

			listing.Label(String.Format("PawnRollSpeedMultiplier".Translate(), PrepareModerately.settings.pawnRollSpeedMultiplier));
			PrepareModerately.settings.pawnRollSpeedMultiplier = (int)listing.Slider(PrepareModerately.settings.pawnRollSpeedMultiplier, 1, 10);

			listing.End();

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() {
			return "PrepareModerately".Translate();
		}
	}
}

/*
Notable classes:
RimWorld.BackstoryCategoryFilter
RimWorld.BackstoryDatabase
RimWorld.Dialog_ScenarioList
RimWorld.Dialog_ScenarioList_Load
RimWorld.Dialog_ScenarioList_Save
RimWorld.NameGenerator
RimWorld.Page_ConfigureStartingPawns
RimWorld.Page_ScenarioEditor
RimWorld.Page_SelectScenario
RimWorld.PawnBioAndNameGenerator
RimWorld.Scenario
RimWorld.ScenarioDef
RimWorld.ScenarioDefOf
RimWorld.ScenarioFiles
RimWorld.ScenarioLister
RimWorld.ScenarioMaker
RimWorld.ScenarioUI
RimWorld.ScenPart
RimWorld.ScenPartDef
RimWorld.ScenPartDefOf
Verse.Listing_ScenEdit
Verse.Pawn
Verse.PawnGenerationRequest
Verse.PawnGenerator

Notable previous commits:
https://github.com/Lakuna/RimWorld-Prepare-Moderately/tree/2ae4be15337c73d0d91f172437bac8f150b85edd (live version)
https://github.com/Lakuna/RimWorld-Prepare-Moderately/tree/7ec44159c68c7a2348ab35a0968471801420d52b (partial update)
*/
