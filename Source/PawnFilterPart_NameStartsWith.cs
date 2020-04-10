using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrepareModerately {
	public class PawnFilterPart_NameStartsWith : PawnFilterPart {
		private readonly string startsWith;

		public PawnFilterPart_NameStartsWith(string startsWith) => this.startsWith = startsWith;

		public override bool Matches(Pawn pawn) => pawn.Name.ToStringFull.StartsWith(this.startsWith);
	}
}
