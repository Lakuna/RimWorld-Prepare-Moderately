using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PrepareModerately {
	class PrepareModerately {
		public static PrepareModerately instance;
		public Page_PrepareModerately page;
		public Page_ConfigureStartingPawns originalPage;

		public static void Instantiate(Page_ConfigureStartingPawns originalPage) {
			PrepareModerately.instance = new PrepareModerately();
			instance.page = new Page_PrepareModerately();
			instance.originalPage = originalPage;
		}
	}
}
