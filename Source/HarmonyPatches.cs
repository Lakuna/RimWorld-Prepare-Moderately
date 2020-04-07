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
			try {
				Log.Message("Building Harmony instance.");
				Harmony harmony = new Harmony("Lakuna.PrepareModerately");

				Log.Message("Getting type configureStartingPawnsPageType.");
				Type configureStartingPawnsPageType = HarmonyPatches.TypeFromName("RimWorld.Page_ConfigureStartingPawns");
				if (configureStartingPawnsPageType == null) { Log.Warning("Failed to add elements to the configure pawns page (type not found)."); return; }

				Log.Message("Getting PreOpen and Postfix methods.");
				MethodBase preOpenMethod = configureStartingPawnsPageType.GetMethod("PreOpen");
				HarmonyMethod preOpenMethodPostfix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("PreOpenPostfix"));

				Log.Message("Patching Page_ConfigureStartingPawns.PreOpen.");
				if (harmony.Patch(preOpenMethod, null, preOpenMethodPostfix) == null) { Log.Warning("Failed to patch the Page_ConfigureStartingPawns.PreOpen method."); }

				Log.Message("Getting DoWindowContents and Postfix methods.");
				MethodBase doWindowContentsMethod = configureStartingPawnsPageType.GetMethod("DoWindowContents");
				HarmonyMethod doWindowContentsMethodPostfix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DoWindowContentsPostfix"));

				Log.Message("Patching Page_ConfigureStartingPawns.DoWindowContents.");
				if (harmony.Patch(doWindowContentsMethod, null, doWindowContentsMethodPostfix) == null) { Log.Warning("Failed to patch the Page_ConfigureStartingPawns.DoWindowContentsPostfix method."); }

				Log.Message("Finished applying Harmony patches.");
			} catch (Exception e) { Log.Warning("Failed to patch for Prepare Moderately (unexpected exception).\n" + e.StackTrace); }
		}

		private static Type TypeFromName(string name) {
			Type type = null;
			try { type = Type.GetType(name, false); } catch (Exception e) { Log.Warning("Could not get type \"" + name + "\"\n" + e.StackTrace); }
			if (type != null) { return type; }
			Assembly[] assemblies;
			try { assemblies = AppDomain.CurrentDomain.GetAssemblies(); } catch (Exception e) { Log.Warning("Could not get a list of assemblies.\n" + e.StackTrace); return null; }
			foreach (Assembly assembly in assemblies) {
				try {
					Type[] types = assembly.GetTypes();
					Type assemblyType = types.FirstOrDefault<Type>((Func<Type, bool>) (innerType => innerType.FullName == name));
					if (assemblyType != null) { return assemblyType; }
					assemblyType = types.FirstOrDefault<Type>((Func<Type, bool>) (innerType => innerType.Name == name));
					if (assemblyType != null) { return assemblyType; }
				} catch (Exception e) {
					string assemblyName = "Unknown";
					try { assemblyName = assembly.GetName().Name; } catch (Exception e2) { Log.Warning("Failed to get assembly name.\n" + e2.StackTrace); }
					Log.Warning("Unable to get list of types from assembly \"" + assemblyName + "\" while searching for type \"" + name + "\".\n" + e.StackTrace);
				}
			}
			return null;
		}

		public static void PreOpenPostfix() { }

		public static void DoWindowContentsPostfix() { }
	}
}
