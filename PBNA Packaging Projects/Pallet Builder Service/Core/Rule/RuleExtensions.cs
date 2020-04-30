using System;
using System.Collections.Generic;
using System.Linq;


namespace ScientificLogistics.PalletBuilder.Core
{
	public static class RuleExtensions
	{
		public static bool IsRuleActive(this List<Rule> ruleList, string ruleCode)
		{
			return ruleList.Any(r => r.IsActive && r.Code.Equals(ruleCode, StringComparison.OrdinalIgnoreCase));
		}
	}
}
