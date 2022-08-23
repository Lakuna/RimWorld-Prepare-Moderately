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
Notable class							Status
RimWorld.BackstoryCategoryFilter		Maybe helpful
RimWorld.BackstoryDatabase				Maybe helpful
RimWorld.Dialog_ScenarioList			TODO
RimWorld.Dialog_ScenarioList_Load		TODO
RimWorld.Dialog_ScenarioList_Save		TODO
RimWorld.NameGenerator					Maybe helpful
RimWorld.Page_ConfigureStartingPawns	TODO
RimWorld.Page_ScenarioEditor			TODO
RimWorld.Page_SelectScenario			TODO
RimWorld.PawnBioAndNameGenerator		Maybe helpful
RimWorld.Scenario						DONE
RimWorld.ScenarioDef					DONE
RimWorld.ScenarioDefOf					DONE
RimWorld.ScenarioFiles					DONE
RimWorld.ScenarioLister					DONE
RimWorld.ScenarioMaker					DONE
RimWorld.ScenarioUI						DONE
RimWorld.ScenPart						DONE
RimWorld.ScenPartDef					DONE
RimWorld.ScenPartDefOf					DONE
Verse.Listing_ScenEdit					DONE
Verse.Pawn								Maybe helpful
Verse.PawnGenerationRequest				Maybe helpful
Verse.PawnGenerator						Maybe helpful

Notable previous commits:
https://github.com/Lakuna/RimWorld-Prepare-Moderately/tree/2ae4be15337c73d0d91f172437bac8f150b85edd (live version)
https://github.com/Lakuna/RimWorld-Prepare-Moderately/tree/7ec44159c68c7a2348ab35a0968471801420d52b (partial update)
*/
