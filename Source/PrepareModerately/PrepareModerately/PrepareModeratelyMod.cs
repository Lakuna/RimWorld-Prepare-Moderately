using System;
using Verse;

[assembly: CLSCompliant(false)]
namespace Lakuna.PrepareModerately {
	public class PrepareModeratelyMod : Mod {
		private static PrepareModeratelySettings settings;

		public static PrepareModeratelySettings Settings => PrepareModeratelyMod.settings;

		public PrepareModeratelyMod(ModContentPack content) : base(content) => PrepareModeratelyMod.settings = this.GetSettings<PrepareModeratelySettings>();
	}
}
