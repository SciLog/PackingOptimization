using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class OrderExtensions
	{
		public static Dictionary<string, List<OrderLine>> GetOrderLinesByType(this Order order, List<Rule> rules)
		{
			Dictionary<string, List<OrderLine>> orderLineDictionary = new Dictionary<string, List<OrderLine>>();

			orderLineDictionary[OrderLineTypeCode.All] = order.OrderLines.ToList();
			orderLineDictionary[OrderLineTypeCode.Chilled] = new List<OrderLine>();
			orderLineDictionary[OrderLineTypeCode.CO2] = new List<OrderLine>();
			orderLineDictionary[OrderLineTypeCode.BIB] = new List<OrderLine>();
			orderLineDictionary[OrderLineTypeCode.Ambient] = new List<OrderLine>();

			bool sprintBIB = rules.IsRuleActive(RuleTypeCode.SprintBib, order);
			bool aggregateCO2 = rules.IsRuleActive(RuleTypeCode.CO2Aggregation, order);

			foreach (OrderLine orderLine in order.OrderLines)
			{
				if (orderLine.Sku.IsChilled)
				{
					orderLineDictionary[OrderLineTypeCode.Chilled].Add(orderLine);
					continue;
				}

				if (aggregateCO2)
				{
					if (orderLine.Sku.IsCO2Supply)
					{
						orderLineDictionary[OrderLineTypeCode.CO2].Add(orderLine);
						continue;
					}
				}

				if (sprintBIB)
				{
					if (orderLine.Sku.ProductMixCode.Equals(ItemTypeCode.BIBProductMix, StringComparison.OrdinalIgnoreCase))
					{
						orderLineDictionary[OrderLineTypeCode.BIB].Add(orderLine);
						continue;
					}
				}

				// If it made it down here, tag it as regular ambient
				orderLineDictionary[OrderLineTypeCode.Ambient].Add(orderLine);
			}


			return orderLineDictionary;
		}
	}
}
