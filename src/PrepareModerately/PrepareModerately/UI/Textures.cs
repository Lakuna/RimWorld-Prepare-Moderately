using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `Verse.TexButton`.
	[StaticConstructorOnStartup]
	internal static class Textures {
		internal static readonly Texture2D DeleteX;

		internal static readonly Texture2D ReorderDown;

		internal static readonly Texture2D ReorderUp;

		internal static readonly Texture2D Warning;

#pragma warning disable CA1810 // Textures must be loaded from the main thread.
		static Textures() {
#pragma warning restore CA1810
#if V1_0 || V1_1 || V1_2
			DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
			ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
			ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
			Warning = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton");
#elif V1_3 || V1_4
			DeleteX = TexButton.DeleteX;
			ReorderDown = TexButton.ReorderDown;
			ReorderUp = TexButton.ReorderUp;
			Warning = TexButton.Info;
#elif V1_5
			DeleteX = TexButton.Delete;
			ReorderDown = TexButton.ReorderDown;
			ReorderUp = TexButton.ReorderUp;
			Warning = TexButton.Info;
#else
			DeleteX = TexButton.Delete;
			ReorderDown = TexButton.ReorderDown;
			ReorderUp = TexButton.ReorderUp;
			Warning = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");
#endif
		}
	}
}
