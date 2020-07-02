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

		public static List<Pallet> GetWithPackage(this List<Pallet> pallets, Package pkg)
		{
			List<Pallet> palletsWithPackage = new List<Pallet>();

			foreach (Pallet p in pallets)
			{
				if (p.ContainsPackage(pkg.PackageId))
				{
					palletsWithPackage.Add(p);
				}
			}

			palletsWithPackage.Sort(new PalletComparator());

			return palletsWithPackage;
		}
	}
}
