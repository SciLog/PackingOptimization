using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Brand
	{
		public int BrandId { get; set; }


		public string TrademarkCode { get; set; }
		public string FlavorCode { get; set; }
		public string CorporateCode { get; set; }
		public string CategoryTypeCode { get; set; }


		public string CaffFlag { get; set; }

		public double BrandJuicePct { get; set; }


		public string BrandShortName { get; set; }
		public string BrandSweetenerCode { get; set; }
		public string BrandProducctColorCode { get; set; }

		public string PungentFlag { get; set; }

		public int SanitationLevelNum { get; set; }
	}
}
