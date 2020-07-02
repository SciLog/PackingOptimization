using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class PackageExtensions
	{
		public static void Dismantle(this Pallet pallet, Order order)
		{
			foreach (PalletItem palletItem in pallet.PalletItems)
			{
				OrderLine orderLine = order
					.OrderLines
					.GetOrderLinesByInventoryId(palletItem.Sku.InventoryId)
					.FirstOrDefault();

				// Original Java Code Does NOT Null Check
				
				orderLine.CaseQuantityRemaining += palletItem.Quantity;
			}

			pallet.NextLayerNumber = 1;
			pallet.UnstablePercentage = 0.0;
			pallet.UnstablePackageIds.Clear();
		}

		// ---

		public static bool IsUnstackable(this Package package, Location location) =>
			location.Unstackables.Any(u => u.BottomPackageId == package.PackageId);

		// ---

		public static int HowManyCasesFit(this Pallet pallet, Package package, double buildToPct, double overflow) =>
			(int)Math.Floor(((buildToPct + overflow) - pallet.GetPercentageFull()) * (package.CasesPerLayer * package.LayersPerPallet));
	}
}
