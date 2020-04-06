using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace PrepareModerately {
	[StaticConstructorOnStartup]
	class HarmonyPatches {
		static HarmonyPatches() {
			Harmony harmony = new Harmony("Lakuna.PrepareModerately");
		}
	}
}
