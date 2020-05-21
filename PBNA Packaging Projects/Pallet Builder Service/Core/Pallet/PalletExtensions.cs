using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class PalletExtensions
	{
		public static void Resequence(this List<Pallet> pallets)
		{
			int index = 1;

			foreach (Pallet p in pallets)
			{
				p.PalletNumber = index;
				index += 1;
			}
		}
	}
}
