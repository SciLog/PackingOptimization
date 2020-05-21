using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class PalletItem
	{
		#region -- Constructors --

		public PalletItem(Order order, Item sku, int caseQuantity, Slotting slotting)
		{
			Order = order;
			Sku = sku;
			Quantity = caseQuantity;
			Layer = 0;

			Slotting = slotting ?? new Slotting();
		}

		#endregion


		#region -- Public Properties --

		public Order Order { get; set; }


		public Item Sku { get; set; }

		public int Quantity { get; set; }

		public int Layer { get; set; }
		public int Column { get; set; }
		public int Cell { get; set; }

		public Slotting Slotting { get; set; }

		public int CustomerDeliverySequency { get; set; }

		public string TransportId { get; set; } 

		
		// GeoBox Enhancements '19

		public string TransportLoadSequence { get; set; }

		public int GroupingCasesPerLayer { get; set; }

		public int GroupingLayersPerPallet { get; set; } 

		
		// GeoBox Enhancements '19

		public string GroupingPalletType { get; set; }

		public string GroupingPalletWeight { get; set; }

		#endregion


		#region -- Public Methods --

		public double CalculatePercentageSkuOfItself() => (double) Quantity / (Sku.Package.CasesPerLayer * Sku.Package.LayersPerPallet);

		#endregion
	}
}
