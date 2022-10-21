using Lakuna.PrepareModerately.Filter;
using Lakuna.PrepareModerately.Filter.Part;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Lakuna.PrepareModerately.UI {
	[StaticConstructorOnStartup]
	public class PawnFilterEditListing : Listing_Standard {
		private readonly PawnFilter filter;

		public PawnFilterEditListing(PawnFilter filter) => this.filter = filter;

		private static readonly float LabelHeight;

		private static readonly Color PartRectBgColor = new Color(1, 1, 1, 0.08f);

		private const float WidgetRowMaxWidth = 72;

		private static readonly Texture2D DeleteXTexture;

		private static readonly Texture2D ReorderUpTexture;

		private static readonly Texture2D ReorderDownTexture;

		private const float GapSize = 4;

#pragma warning disable CA1810 // Textures must be loaded from the main thread.
		static PawnFilterEditListing() {
#pragma warning restore CA1810
#if V1_0 || V1_1 || V1_2
			DeleteXTexture = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
			ReorderUpTexture = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp");
			ReorderDownTexture = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown");
#else
			DeleteXTexture = TexButton.DeleteX;
			ReorderUpTexture = TexButton.ReorderUp;
			ReorderDownTexture = TexButton.ReorderDown;
#endif
			LabelHeight = new float[] {
				DeleteXTexture.height,
				ReorderUpTexture.height,
				ReorderDownTexture.height,
				Text.LineHeight
			}.Max();
		}

		public Rect GetPawnFilterPartRect(PawnFilterPart part, float height) {
			if (part == null) {
				throw new ArgumentNullException(nameof(part));
			}

			Rect rect = this.GetRect(LabelHeight + height);
			Widgets.DrawBoxSolid(rect, PartRectBgColor);
			WidgetRow widgetRow = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, WidgetRowMaxWidth, 0);

			if (part.Def.PlayerAddRemovable && widgetRow.ButtonIcon(DeleteXTexture, null, GenUI.SubtleMouseoverColor)) {
				this.filter.RemovePart(part);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(ReorderUpTexture)) {
				this.filter.Reorder(part, ReorderDirection.Up);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}

			if (this.filter.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(ReorderDownTexture)) {
				this.filter.Reorder(part, ReorderDirection.Down);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}

			Rect labelRect = new Rect(rect.x + WidgetRowMaxWidth, rect.y, rect.width - WidgetRowMaxWidth, LabelHeight);
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(labelRect, part.Label.CapitalizeFirst());
			Text.Anchor = TextAnchor.UpperLeft; // Text anchor must end on upper left.

			this.Gap(GapSize);

			return new Rect(rect.x, rect.y + labelRect.height, rect.width, rect.height - labelRect.height).Rounded();
		}
	}
}
