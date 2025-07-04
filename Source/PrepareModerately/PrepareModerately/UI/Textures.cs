﻿using UnityEngine;
using Verse;

namespace Lakuna.PrepareModerately.UI {
	// Based on `Verse.TexButton`.
	[StaticConstructorOnStartup]
	internal static class Textures {
		internal static readonly Texture2D DeleteX;

		internal static readonly Texture2D ReorderUp;

		internal static readonly Texture2D ReorderDown;

#pragma warning disable CA1810 // Textures must be loaded from the main thread.
		static Textures() {
#pragma warning restore CA1810
#if V1_0 || V1_1 || V1_2
			DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
			ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
			ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
#elif V1_3 || V1_4
			DeleteX = TexButton.DeleteX;
			ReorderUp = TexButton.ReorderUp;
			ReorderDown = TexButton.ReorderDown;
#else
			DeleteX = TexButton.Delete;
			ReorderUp = TexButton.ReorderUp;
			ReorderDown = TexButton.ReorderDown;
#endif
		}
	}
}
