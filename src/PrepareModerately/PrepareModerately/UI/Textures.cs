using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `Verse.TexButton`.
	[StaticConstructorOnStartup]
	public static class Textures {
#if V1_0 || V1_1 || V1_2
		public static readonly Texture2D Delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
#elif V1_3 || V1_4
		public static readonly Texture2D Delete = TexButton.DeleteX;
#elif V1_5 || V1_6
		public static readonly Texture2D Delete = TexButton.Delete;
#else
		public static readonly Texture2D? Delete = TexButton.Delete;
#endif

#if V1_0 || V1_1 || V1_2
		public static readonly Texture2D ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
#elif V1_3 || V1_4 || V1_5 || V1_6
		public static readonly Texture2D ReorderDown = TexButton.ReorderDown;
#else
		public static readonly Texture2D? ReorderDown = TexButton.ReorderDown;
#endif

#if V1_0 || V1_1 || V1_2
		public static readonly Texture2D ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
#elif V1_3 || V1_4 || V1_5 || V1_6
		public static readonly Texture2D ReorderUp = TexButton.ReorderUp;
#else
		public static readonly Texture2D? ReorderUp = TexButton.ReorderUp;
#endif

#if V1_0 || V1_1 || V1_2
		public static readonly Texture2D Warning = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton");
#elif V1_3 || V1_4 || V1_5
		public static readonly Texture2D Warning = TexButton.Info;
#elif V1_6
		public static readonly Texture2D Warning = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");
#else
		public static readonly Texture2D? Warning = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");
#endif
	}
}
