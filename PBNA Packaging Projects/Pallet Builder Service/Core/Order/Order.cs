using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Order
	{
		#region -- Constructors --

		public Order()
		{
			OrderLines = new List<OrderLine>();
			ReasonCode = "";
			IsExtracted = true;
			OrderComments = new List<String>();
			IsStandardPalletOnly = true;
		}

		#endregion


		#region -- Public Properties --

		public string OrderId { get; set; }					

		public string JobId { get; set; }					

		public string IdTypeCode { get; set; }

		public string OrderTypeCode { get; set; }			

		public List<OrderLine> OrderLines { get; set; }

		public int RouteId { get; set; }

		public Customer Customer { get; set; } = new Customer();

		public int StopNumber { get; set; }

		public Location Location { get; set; } = new Location();
		public Location DeliveryLocation { get; set; } = new Location();

		public string DeliveryMethodCode { get; set; }
		public string OriginalDeliveryMethodCode { get; set; }


		// Updated delivery method

		public string OrganizationDeliveryMethodCode { get; set; }


		// Original delivery method

		public bool IsFlippedDeliveryMethodCode { get; set; }


		// There is a mismatch between DMC of plto and plto_prfl

		public string OrderType { get; set; }

		public int TotalCaseQuantity { get; set; }

		public int CycleTimeHours { get; set; }

		public DateTime CreateDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public DateTime ExpectedDate { get; set; }
		public DateTime BuildDate { get; set; }
		public DateTime DbDate { get; set; }


		// Current time from database

		public DateTime LocalDate { get; set; }

		public List<OrderSourcingMatrix> OrderSourcingMatrices { get; set; }

		public string StatusCode { get; set; }
		public string ReasonCode { get; set; }

		public int LoadId { get; set; }

		public DateTime UpdateDateTime { get; set; }

		public string ReasonText { get; set; }

		public bool IsContentLocked { get; set; }
		public bool IsRescheduledLocked { get; set; }
		public bool IsCanceledLocked { get; set; }

		public int Priority { get; set; }

		public DateTime LatestBuildByTime { get; set; }

		public Location PrimaryBuildLocation { get; set; } = new Location();

		public string UpdateDatetimeString { get; set; }

		public int UpdateCount { get; set; }

		public string SourceServerName { get; set; }

		public bool IsExtracted { get; set; }

		public bool IsPickable { get; set; }

		public bool IsCritical { get; set; }

		public bool CfSet { get; set; }

		public DateTime SourceUpdateDatetime { get; set; }

		public DateTime nisShipDate { get; set; }

		public bool IsReleased { get; set; }

		protected int TotalNumberOfPallets { get; set; }

		public bool allowReusePallets { get; set; }

		public bool IsReleasedForSpecificDeliveryDateToWMS { get; set; }

		public DefaultNotification DefaultNotification { get; set; }

		public bool IsTransportAssigned { get; set; }


		// For BOL's
		public DateTime ShipDate { get; set; }

		public bool IsStandardPalletOnly { get; set; }

		public bool IsBuildIdOverriden { get; set; }


		// Flag determines if the build location has been overwritten
		public bool IsApbExempt { get; set; }


		// Flag determines if the order can be APB'd
		public bool IsPalletizerRun { get; set; }

		// public kit As Kit

		public List<string> OrderComments { get; set; }

		public bool IsMerchandisePalletOrder { get; set; }
	
		public bool ShouldUpdatePPrflDmc { get; set; }

		public string OriginalStatusCode { get; set; }

		public string LoadTypeCode { get; set; }

		protected bool IsBabOrderIndicator { get; set; }

		#endregion

		#region -- Private Properties --

		private int LocationId
		{
			set { Location.LocationId = value; }
		}


		private int DeliveryLocationId
		{
			set { DeliveryLocation.LocationId = value; }
		}

		private int PrimaryBuildLocationId
		{
			set { PrimaryBuildLocation.LocationId = value; }
		}

		private int CustomerId
		{
			set { Customer.CustomerId = value; }
		}
		private bool CustomerIsStandardPalletOnly
		{
			set { Customer.IsStandardPalletOnly = value; }
		}
		private bool CustomerIsAdvancedShippingNotice
		{
			set { Customer.IsAdvancedShippingNotice = value; }
		}
		private bool CustomerIsMerchantAccount
		{
			set { Customer.IsMerchantAccount = value; }
		}
		private string CustomerPalletizationOption
		{
			set { Customer.PalletizationOption = value; }
		}

		#endregion


		#region -- Public Methods --

		public void ConvertCustomerOrderableUnitstoCases(bool truncateUnevenQty)
		{
			// Do not convert cases if MerchPallet
			if (IsMerchandisePalletOrder) return;

			//If the delivery method is BOL no conversion necessary, just copy customer orderable units.
			//Else convert customer orderable units to cases.
			foreach (OrderLine orderLine in OrderLines)
			{
				if (DeliveryMethodCode == PalletBuilder.DeliveryMethodCode.BOL)
				{
					orderLine.CaseQuantity = orderLine.CustomerOrderableUnits;
					orderLine.CaseQuantityRemaining = orderLine.CustomerOrderableUnits;
				}
				else
				{
					orderLine.ConvertCustomerOrderableUnitstoCases(truncateUnevenQty);
				}
			}
		}

		public void CombineSameSkuOrderLines()
		{
			List<int> combinedSkus = new List<int>();

			for (int i = 0; i < OrderLines.Count; i++)
			{
				OrderLine orderLine1 = OrderLines[i];

				if (!combinedSkus.Contains(orderLine1.Sku.InventoryId))
				{
					for (int j = i + 1; j < OrderLines.Count; j++)
					{
						OrderLine orderLine2 = OrderLines[j];

						if (orderLine1.Sku.InventoryId == orderLine2.Sku.InventoryId)
						{
							orderLine1.CaseQuantityRemaining += orderLine2.CaseQuantityRemaining;
							orderLine2.CaseQuantityRemaining = 0;
							
							if (!combinedSkus.Contains(orderLine1.Sku.InventoryId))
							{
								combinedSkus.Add(orderLine1.Sku.InventoryId);
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
