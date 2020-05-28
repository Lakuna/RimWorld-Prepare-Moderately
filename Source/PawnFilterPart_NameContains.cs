﻿using UnityEngine;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NameContains : PawnFilterPart {
		private string contains;

		public PawnFilterPart_NameContains() => this.label = "Name contains:";

		public override void DoEditInterface(Listing_PawnFilter list) {
			Rect rect = list.GetPawnFilterPartRect(this, RowHeight);
			this.contains = Widgets.TextArea(rect, this.contains);
		}

		public override bool Matches(Pawn pawn) => pawn.Name.ToStringFull.Contains(this.contains);

		public override string ToLoadableString() => this.GetType().Name + " " + this.contains;

		public override void FromLoadableString(string s) {
			string[] parts = s.Split(' ');
			this.contains = parts[1];
		}
	}
}
