using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class OrderExtensions
	{
		public static Dictionary<OrderLine, Slotting> GetItemsInSlotting(this List<OrderLine> orderLines, List<Slotting> slottings)
		{
			//Figure out the Order Lines that are contained in the cell
			Dictionary<OrderLine, Slotting> inCell = new Dictionary<OrderLine, Slotting>();

			//List<OrderLine> inCell = new ArrayList<OrderLine>();
			foreach (OrderLine orderLine in orderLines)
			{
				if (slottings.Contains(orderLine.Sku.InventoryId))
				{
					inCell.Add(orderLine, slottings.FindFirst(orderLine.Sku.InventoryId));
				}
			}
			//Sort the Map by pick sequence
			List<KeyValuePair<OrderLine, Slotting>> sortedEntries = new List<KeyValuePair<OrderLine, Slotting>>(inCell);
			
			sortedEntries.Sort(new OrderLineSlottingComparator());
			inCell.Clear();
			
			foreach (KeyValuePair<OrderLine, Slotting> entry in sortedEntries)
			{
				inCell.Add(entry.Key, entry.Value);
			}

			return inCell;
		}

		// ---

		public static int IsSlottedOnSameSide(this List<OrderLine> orderLines, List<List<Slotting>> sidewiseSlotting)
		{
			int slottedOnSameSide = -1;

			List<Slotting> superSetSlotting = sidewiseSlotting[1];
			List<Slotting> subSetSlotting = sidewiseSlotting[0];
			
			foreach (OrderLine orderLine in orderLines)
			{
				if (orderLine.Sku.GetSlotting(subSetSlotting) == null)
				{
					slottedOnSameSide = -1;
				}
				else
				{
					slottedOnSameSide = 0;
				}
				if (slottedOnSameSide < 0)
				{
					break;
				}
			}
			if (slottedOnSameSide < 0)
			{
				foreach (OrderLine orderLine in orderLines)
				{
					if (orderLine.Sku.GetSlotting(superSetSlotting) == null)
					{
						slottedOnSameSide = -1;
					}
					else
					{
						slottedOnSameSide = 1;
					}
					if (slottedOnSameSide < 0)
					{
						break;
					}
				}
			}

			return slottedOnSameSide;
		}

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

		public static IEnumerable<OrderLine> GetByPackageId(this List<OrderLine> orderLines, int packageId) =>
			orderLines
				.Where(ol => ol.Sku.Package.PackageId == packageId);

		public static IEnumerable<OrderLine> GetBySlotting(this List<OrderLine> orderLines, List<Slotting> slottings) =>
			orderLines
				.Where(ol => slottings.Contains(ol.Sku.InventoryId));

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

		public static double CalculateTotalPercentageSkuOfItself(this List<OrderLine> orderLines) =>
			orderLines.Sum(ol => ol.CalculatePercentageSkuOfItself());

		public static int CalculateNumberOfLayers(this OrderLine orderLine) =>
			orderLine.CaseQuantityRemaining / orderLine.Sku.Package.CasesPerLayer;

		// ---

		public static List<OrderLine> GetCombosForCaseQty(this List<OrderLine> packageOrderLines, List<Slotting> slotting, int caseQuantity)
		{
			List<OrderLine> itemList = new List<OrderLine>();

			if (packageOrderLines.Count != 0 && (caseQuantity > 0))
			{

				//Try to find a SKU with a case quantity equal to the remainder from making layers
				foreach (OrderLine packageOrderLine in packageOrderLines)
				{
					if (slotting.Contains(packageOrderLine.Sku.InventoryId) && 
						packageOrderLine.CaseQuantity == caseQuantity)
					{
						itemList.Add(packageOrderLine);
						return itemList;
					}
				}

				//Try to find 2 SKUs with a combined case quantity equal to the remainder from making layers;
				for (int i = 0; i < packageOrderLines.Count; i++)
				{
					OrderLine packageOrderLine = packageOrderLines[i];

					if (slotting.Contains(packageOrderLine.Sku.InventoryId))
					{
						for (int j = i + 1; j < packageOrderLines.Count; j++)
						{
							OrderLine pkgOrdLine2 = packageOrderLines[j];

							if (slotting.Contains(pkgOrdLine2.Sku.InventoryId))
							{
								int combinedQuantity = packageOrderLine.CaseQuantityRemaining + pkgOrdLine2.CaseQuantityRemaining;

								if (combinedQuantity == caseQuantity)
								{
									itemList.Add(packageOrderLine);
									itemList.Add(pkgOrdLine2);

									return itemList;
								}
							}
						}
					}
				}

				//Try to find 3 SKUs with a combined case quantity equal to the remainder from making layers
				for (int i = 0; i < packageOrderLines.Count; i++)
				{
					OrderLine packageOrderLine = packageOrderLines[i];

					if (slotting.Contains(packageOrderLine.Sku.InventoryId))
					{
						for (int j = i + 1; j < packageOrderLines.Count; j++)
						{
							OrderLine pkgOrdLine2 = packageOrderLines[j];

							if (slotting.Contains(pkgOrdLine2.Sku.InventoryId))
							{
								for (int k = j + 1; k < packageOrderLines.Count; k++)
								{
									OrderLine pkgOrdLine3 = packageOrderLines[k];
									if (slotting.Contains(pkgOrdLine3.Sku.InventoryId))
									{
										int combinedQty = 
											packageOrderLine.CaseQuantityRemaining + 
											pkgOrdLine2.CaseQuantityRemaining + 
											pkgOrdLine3.CaseQuantityRemaining;

										if (combinedQty == caseQuantity)
										{
											itemList.Add(packageOrderLine);
											itemList.Add(pkgOrdLine2);
											itemList.Add(pkgOrdLine3);

											return itemList;
										}
									}
								}
							}
						}
					}
				}

				//If no combo is found just return the biggest one
				packageOrderLines.Sort(new OrderLineComparator());
				itemList.Add(packageOrderLines.Last());

				return itemList;
			}

			return itemList;
		}


	}
}
