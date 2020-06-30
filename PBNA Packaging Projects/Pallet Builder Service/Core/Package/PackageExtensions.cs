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
	}
}
