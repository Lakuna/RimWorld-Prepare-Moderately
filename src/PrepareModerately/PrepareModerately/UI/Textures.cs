using UnityEngine;

using Verse;

namespace Lakuna.PrepareModerately.Ui {
	// Based on `Verse.TexButton`.
	[StaticConstructorOnStartup]
	internal static class Textures {
		internal static readonly Texture2D _delete;

		internal static readonly Texture2D _reorderDown;

		internal static readonly Texture2D _reorderUp;

		internal static readonly Texture2D _warning;

#pragma warning disable CA1810 // Textures must be loaded from the main thread.
		static Textures() {
#pragma warning restore CA1810
#if V1_0 || V1_1 || V1_2
			_delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
			_reorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
			_reorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
			_warning = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton");
#elif V1_3 || V1_4
			_delete = TexButton.DeleteX;
			_reorderDown = TexButton.ReorderDown;
			_reorderUp = TexButton.ReorderUp;
			_warning = TexButton.Info;
#elif V1_5
			_delete = TexButton.Delete;
			_reorderDown = TexButton.ReorderDown;
			_reorderUp = TexButton.ReorderUp;
			_warning = TexButton.Info;
#else
			_delete = TexButton.Delete;
			_reorderDown = TexButton.ReorderDown;
			_reorderUp = TexButton.ReorderUp;
			_warning = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");
#endif
		}
	}
}
