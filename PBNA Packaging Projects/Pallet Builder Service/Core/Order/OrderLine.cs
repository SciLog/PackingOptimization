using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderLine
	{
		#region -- Constructors --

		public OrderLine()
		{
		}

		#endregion


		#region -- Public Properties --

		public string OrderId { get; set; }

		public string IdTypeCode { get; set; }

		public int OrderLineId { get; set; }

		public Item Sku { get; set; } = new Item();

		public int CustomerOrderableUnits { get; set; }

		public int CaseQuantity { get; set; }

		public string LineActivityCode { get; set; }
		public DateTime LineCreateDate { get; set; }

		public bool IsDirectLoad { get; set; }

		public int PalletCaseQuantity { get; set; }

		public int UpdateCount { get; set; }

		public int CaseQuantityRemaining { get; set; }

		public List<SplitOrderLine> SplitOrderLines { get; set; }

		public bool IsCritical { get; set; }

		#endregion

		#region -- Private Properties --

		// --- For Dapper SQL ---

		private int SkuInventoryId
		{
			set { Sku.InventoryId = value; }
		}

		private string SkuInventoryTypeCode
		{
			set { Sku.InventoryTypeCode = value; }
		}

		private int SkuPackageId
		{
			set { Sku.Package.PackageId = value; }
		}

		private string SkuPalletTypeCode
		{
			set { Sku.PalletTypeCode = value; }
		}

		private string SkuName
		{
			set { Sku.Name = value; }
		}

		private string SkuSupplyTypeCode
		{
			set { Sku.SupplyTypeCode = value; }
		}

		private bool SkuIsChilled
		{
			set { Sku.IsChilled = value; }
		}

		private string SkuProductMixCode
		{
			set { Sku.ProductMixCode = value; }
		}


		#endregion



		#region -- Public Methods --

		public double CalculatePercentageSkuOfItself() => (double)CaseQuantityRemaining / (Sku.Package.CasesPerLayer * Sku.Package.LayersPerPallet);
		public double CalculatePercentageSkuOfItself(int caseQuantity) => (double)caseQuantity / (Sku.Package.CasesPerLayer * Sku.Package.LayersPerPallet);

		// ---

		public void ConvertCustomerOrderableUnitstoCases(bool truncateUnevenQuantity)
		{
			if (!truncateUnevenQuantity)
			{
				double remainder = CustomerOrderableUnits % Sku.CustomerOrderableUintsPerCase;

				if (remainder != 0)
				{
					throw new Exception($"Inven {Sku.InventoryId} with customer orderable units " +
							$"{CustomerOrderableUnits} does not convert to an even case quantity using {Sku.CustomerOrderableUintsPerCase}" +
							" customer orderable units per case.");
				}
			}

			CaseQuantity = (int)(CustomerOrderableUnits / Sku.CustomerOrderableUintsPerCase);

			// if converted orderable unit is 0 and this is not a pure credit line, make sure it is at least one unit.
			// i.e., if one 3G BIB (3 units per case) was ordered, the case qty would return 0. We must make it at least one case.  
			//if (this.ordLineId != 0 && this.caseQty == 0)
			//this.caseQty = 1;

			CaseQuantityRemaining = CaseQuantity;
		}


		#endregion
	}
}
