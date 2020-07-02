using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderLineComparatorMXLP : IComparer<OrderLine> 
{


	public int Compare(OrderLine orderLine1, OrderLine orderLine2)
	{
		int result = orderLine1.CaseQuantityRemaining.CompareTo(orderLine2.CaseQuantityRemaining);
		
		if (result == 0)
		{
			result = orderLine1.Sku.BrandLayerSequence.CompareTo(
					orderLine2.Sku.BrandLayerSequence);
		}
		else
		{
			//if orderLine quantities are different then return back equal.  The reason being I want to preserve the existing
			//order of the orderLines.  Change the order only if quantities are equal
			result = 0;
		}
		return result;
	}

}
}
