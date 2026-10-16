using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `Verse.TexButton`.
	[StaticConstructorOnStartup]
	internal static class Textures {
		internal static readonly Texture2D s_delete;

		internal static readonly Texture2D s_reorderDown;

		internal static readonly Texture2D s_reorderUp;

		internal static readonly Texture2D s_warning;

#pragma warning disable CA1810 // Textures must be loaded from the main thread.
		static Textures() {
#pragma warning restore CA1810
#if V1_0 || V1_1 || V1_2
			s_delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
			s_reorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
			s_reorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
			s_warning = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton");
#elif V1_3 || V1_4
			s_delete = TexButton.DeleteX;
			s_reorderDown = TexButton.ReorderDown;
			s_reorderUp = TexButton.ReorderUp;
			s_warning = TexButton.Info;
#elif V1_5
			s_delete = TexButton.Delete;
			s_reorderDown = TexButton.ReorderDown;
			s_reorderUp = TexButton.ReorderUp;
			s_warning = TexButton.Info;
#else
			s_delete = TexButton.Delete;
			s_reorderDown = TexButton.ReorderDown;
			s_reorderUp = TexButton.ReorderUp;
			s_warning = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");
#endif
		}
	}
}
