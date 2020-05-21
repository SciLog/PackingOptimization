using System;
using System.Collections.Generic;
using System.Linq;


namespace ScientificLogistics.PalletBuilder.Core
{
	public static class RuleExtensions
	{
		public static bool IsRuleActive(this List<Rule> rules, string ruleCode)
		{
			return rules.Any(r => r.IsActive && r.Code.Equals(ruleCode, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsRuleActive(this List<Rule> rules, string ruleTypeCode, Order order)
		{
			return rules.Any(r =>
				r.Code.Equals(ruleTypeCode, StringComparison.OrdinalIgnoreCase) &&
				r.LocationId == order.Location.LocationId &&
				r.OrderType.Equals(order.OrderTypeCode, StringComparison.OrdinalIgnoreCase) &&
				r.DeliveryMethodCode.Equals(order.DeliveryMethodCode, StringComparison.OrdinalIgnoreCase) &&
				r.IsActive);
		}

		public static bool IsRuleActive(this List<Rule> rules, string ruleTypeCode, Order order, int locationId, 
			string orderTypeCode, string deliveryMethodCode)
		{
			return rules.Any(r =>
				r.Code.Equals(ruleTypeCode, StringComparison.OrdinalIgnoreCase) &&
				r.LocationId == order.Location.LocationId &&
				r.OrderType.Equals(order.OrderTypeCode, StringComparison.OrdinalIgnoreCase) &&
				r.DeliveryMethodCode.Equals(order.DeliveryMethodCode, StringComparison.OrdinalIgnoreCase) &&
				r.IsActive);
		}

	}
}
