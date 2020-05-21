using ScientificLogistics.PalletBuilder.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public class OrderProcessor
	{
		public void ProcessOrders(int locationId)
		{
			RuleRepository ruleRepository = new RuleRepository();

			
			// Locking Orders if needed		
			
			List<Rule> ruleList = ruleRepository.GetRulesForLocation(locationId);

			if (ruleList.IsRuleActive(RuleTypeCode.InductCheckDeliveryDate))
			{
				//orderDelegate.lockOrdersAfterDispCutoff(locId, rulesDelegate.getDelyMethodsHavingRuleActive(Constants.RULE_ID_INDUCT_CHECK_DELVY_DTE, rules));
			}

			//Check to see if the location is an automation site
			if (ruleList.IsRuleActive(RuleTypeCode.Automation))
			{
				//automationDelegate.copyAutomationPalletSol(locId);
				//automationDelegate.copyAutomationTransportSol(locId);
			}
		}
	}
}
