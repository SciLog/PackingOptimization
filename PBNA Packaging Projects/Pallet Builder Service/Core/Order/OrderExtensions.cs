using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

		public static Dictionary<OrderLine, Slotting> GetSlottings(this List<OrderLine> orderLines, List<Slotting> slottings) =>
			orderLines
				.Where(ol => slottings.Contains(ol.Sku.InventoryId))						// Find Order Lines With Slotting
				.ToDictionary(ol => ol, ol => slottings.FindFirst(ol.Sku.InventoryId));		// Convert to Dictionary		

		public static IEnumerable<Package> GetDistinctPackages( this List<OrderLine> orderLines)
		{
			// Linq Distinct() Method does not take a lambda. Use GroupBy instead.

			return orderLines
				.Select(ol => ol.Sku.Package)
				.GroupBy(
					p => p.PackageId,
					(key, group) => group.First())
				.OrderBy(p => p.Priority);
		}

		public static IEnumerable<OrderLine> GetOrderLinesByInventoryId(this List<OrderLine> orderLines, int inventoryId) =>
			orderLines
				.Where(ol => ol.Sku.InventoryId == inventoryId);

		public static IEnumerable<OrderLine> GetRemainingOrderLines(this List<OrderLine> orderLines) =>
			orderLines
				.Where(ol => ol.CaseQuantityRemaining > 0);

		public static IEnumerable<OrderLine> GetRemainingOrderLinesByPackageId(this List<OrderLine> orderLines, int packageId) =>
			orderLines
				.GetRemainingOrderLines()
				.Where(ol => ol.Sku.Package.PackageId == packageId);

		public static IEnumerable<OrderLine> GetRemainingOrderLinesByPackageId(this List<OrderLine> orderLines, int packageId, List<Slotting> slottings) =>
			orderLines.GetRemainingOrderLinesByPackageId(packageId)
				.Where(ol => slottings.Contains(ol.Sku.InventoryId));
				
		public static double CalculatePercentageSkuOfItself(this OrderLine orderLine) =>
			(double) orderLine.CaseQuantityRemaining / 
			(double) (orderLine.Sku.Package.CasesPerLayer * orderLine.Sku.Package.LayersPerPallet);

		public static int CalculateNumberOfLayers(this OrderLine orderLine) =>
			orderLine.CaseQuantityRemaining / orderLine.Sku.Package.CasesPerLayer;

	}
}
