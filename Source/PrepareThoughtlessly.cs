using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace PrepareThoughtlessly {
	[StaticConstructorOnStartup]
	public class PrepareThoughtlessly {
		static PrepareThoughtlessly() { new Harmony("Lakuna.PrepareThoughtlessly").PatchAll(Assembly.GetExecutingAssembly()); }
	}
}
