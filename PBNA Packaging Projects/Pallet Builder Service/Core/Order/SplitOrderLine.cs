using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class SplitOrderLine
	{
		public int BuildLocationId { get; set; }

		public int PickId { get; set; }

		public int CaseQuantity { get; set; }

		public int SplitNumber { get; set; }
	}
}
