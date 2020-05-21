using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Customer
	{
		public int CustomerId { get; set; }
		public int GroupId { get; set; }

		public bool IsAdvancedShippingNotice { get; set; }
		public bool IsMerchantAccount { get; set; }
		public bool IsNight { get; set; }
		
		public bool IsSideload { get; set; }	// DBay
		public bool IsBulk { get; set; }
			
		public bool IsStandardPalletOnly { get; set; }

		public bool IsMerchangeAccount { get; set; }

		public string PalletizationOption { get; set; } = PalletizationOptionCode.LP_AND_HP;
	}
}
