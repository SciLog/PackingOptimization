using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Rule
	{
		public string Code { get; set; }						// code
		public int LocationId { get; set; }						// locId	
		public string DeliveryMethodCode { get; set; }          // deliveryMethodCode
		public string OrderType { get; set; }				    // orderType
		public string ActiveFlag { get; set; }					// activeFlg


		public bool IsActive => ActiveFlag?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false;			
	}
}
