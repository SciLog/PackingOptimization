using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderLineComparator : IComparer<OrderLine>
	{
		public int Compare(OrderLine orderLine1, OrderLine orderLine2)
		{
			return orderLine1.CalculatePercentageSkuOfItself().CompareTo(orderLine2.CalculatePercentageSkuOfItself());
		}

	}
}
