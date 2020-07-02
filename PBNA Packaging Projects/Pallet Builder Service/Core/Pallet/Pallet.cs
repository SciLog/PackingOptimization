using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Pallet
	{
		#region -- Instance Variables ---

		private Item _defaultPallet;
		private bool _isElementOfMultiBaySet;

		#endregion

		#region -- Construtors --

		public Pallet()
		{
			_defaultPallet = new Item()
			{
				Height = 5.25,
				Width = 40.0,
				Length = 48.0
			};
		}

		public Pallet(Item defaultPallet)
		{
			_defaultPallet = defaultPallet;
		}

		#endregion


		#region -- Public Properties --

		public PalletType PalletType { get; set; } = PalletType.Unknown;

		public List<PalletItem> PalletItems { get; } = new List<PalletItem>();
		public List<PalletItem> DetailPalletItems { get; } = new List<PalletItem>();
		public List<Order> Orders { get; } = new List<Order>(); 
		public List<int> UnstablePackageIds { get; } = new List<int>();
		public List<Layer> Layers { get; } = new List<Layer>();

		public int NextLayerNumber { get; set; } = 1;
	
		public double UnstablePercentage { get; set; }

		public int PalletNumber { get; set; } = -1;

		public double PercentageFullFullLayer { get; set; }
		public double PercentageFullMXL { get; set; }
		public double PercentageFullEaches { get; set; }

		public double PackageTypeCount { get; set; }
		public double PackageTypeCountFullLayer { get; set; }
		public double PackageTypeCountMXL { get; set; }
		public double PackageCountEaches { get; set; }

		public double Height { get; set; }
		public double Volume { get; set; }



		//public Item DefaultItem { get; set; }
		//public Item Item { get; set; }


		//public List<Layer> Layers { get; } = new List<Layer>();

		#endregion



		public bool CanAccomodateLayersOfOnePkg(double buildToPercentage, double overFlow, params Layer[] layers)
		{
			bool yes = false;

			if ((layers != null) && (layers.Length > 0))
			{
				Package pkg = layers[0].Package;

				double parPct = (double) layers.Length / pkg.LayersPerPallet;
				double comPct = GetPercentageFull() + parPct;

				yes = ((comPct < buildToPercentage) || (comPct >= buildToPercentage && comPct <= (buildToPercentage + overFlow)));
			}

			return yes;
		}

		public bool WouldAffectStackability(Layer layer)
		{
			bool xx = false;
			int gpis = (layer.PickSequence ?? 0 / 100);
			
			foreach (Layer l in Layers)
			{
				int pis = (l.PickSequence ?? 0 / 100);
				
				xx = (gpis == 3) && (pis == gpis) && 
					(l.Package.PackageId != layer.Package.PackageId);
				
				if (xx) { break; };
			}
			
			return xx;
		}

		public List<int> GetPackagesOnPallet()
		{
			return Layers
				.Select(l => l.Package.PackageId)
				.Distinct()
				.ToList();
		}

		private PalletItem GetPalletItemByInventoryId(int inventoryId, string orderId) =>
			PalletItems.FirstOrDefault(pi =>
				(pi.Layer == 0) &&
				(pi.Sku.InventoryId == inventoryId) &&
				(pi.Order.OrderId == orderId));

		public bool AddItemToPalletWithoutCheck(Order order, OrderLine ordLine, int caseQty, Slotting slotting)
		{
			Item sku = ordLine.Sku;
			PalletItem palletItem = new PalletItem(order, sku, caseQty, slotting);

			// If item is a layer item set the layer number
			if(slotting?.PickMethodCode == PickMethodCode.FullLayer ||
				slotting?.PickMethodCode == PickMethodCode.MixedLayer)
			{
				palletItem.Layer = NextLayerNumber;
			}
			else
			{
				// Else check if the inven already exists on this pallet 
				PalletItem palletItemOnPallet = GetPalletItemByInventoryId(sku.InventoryId, order.OrderId);

				if (palletItemOnPallet != null)
				{
					palletItem = palletItemOnPallet;
				}
			}


			// If inven already exists on this pallet add to it's quantity
			if (PalletItems.Contains(palletItem))
			{
				palletItem.Quantity += caseQty;
			}
			else
			{
				// Else add the item to the pallet
				PalletItems.Add(palletItem);
			}

			if (!Orders.Contains(order))
			{
				Orders.Add(order);
			}

			return true;
		}

		public void ReverseLayers()
		{
			PalletItems.Reverse();

			int previousLayerNumber = 0;
			int layerNumber = 1;
			
			foreach (PalletItem palletItem in PalletItems)
			{
				if (previousLayerNumber != palletItem.Layer)
				{
					layerNumber += 1;
				}

				previousLayerNumber = palletItem.Layer;
				palletItem.Layer = layerNumber;
			}
		}

		public double GetPercentageFull()
		{
			double percentFull = 0;

			foreach (PalletItem palletItem in PalletItems)
			{
				percentFull += palletItem.CalculatePercentageSkuOfItself();
			}

			return percentFull;
		}

		public int HowManyCasesFit(double buildToPercentage, double overflow, int casesPerLayer, int layersPerPallet)
		{
			return (int)Math.Floor(
				(buildToPercentage + overflow - GetPercentageFull()) *
				(casesPerLayer * layersPerPallet));
		}

		public bool AddItemToPallet(Order order, OrderLine orderLine, int caseQuantity, Slotting slotting, double buldToPercentage, 
			double overflow, bool stable, bool enforceStability, int maxUnstablePackages, double maxUnstablePercentage)
		{
			bool isAdded = false;
			
			PalletItem palletItem = new PalletItem(order, orderLine.Sku, caseQuantity, slotting);

			if(slotting?.PickMethodCode == PickMethodCode.FullLayer ||      // If item is a layer item set the layer number
				slotting?.PickMethodCode == PickMethodCode.MixedLayer)
			{
				
				palletItem.Layer = NextLayerNumber;
			}
			else															// Else check if the inven already exists on this pallet    
			{
				PalletItem palletItemOnPallet = this.GetPalletItemByInventoryId(orderLine.Sku.InventoryId, order.OrderId);
				
				if (palletItemOnPallet != null)
				{
					palletItem = palletItemOnPallet;
				}
			}

			if (CanAdd(orderLine, caseQuantity, buldToPercentage, overflow, enforceStability, maxUnstablePackages, maxUnstablePercentage))
			{
				if (PalletItems.Contains(palletItem))       // If inven already exists on this pallet add to it's quantity
				{
					palletItem.Quantity += caseQuantity;
				}
				else										// Else add the item to the pallet
				{
					PalletItems.Add(palletItem);
				}

				double itemPercentage = orderLine.CalculatePercentageSkuOfItself(caseQuantity);

				if (!stable)
				{
					UnstablePercentage += itemPercentage;

					if (!UnstablePackageIds.Contains(orderLine.Sku.Package.PackageId))
					{
						UnstablePackageIds.Add(orderLine.Sku.Package.PackageId);
					}
				}

				if (!Orders.Contains(order))
				{
					Orders.Add(order);
				}

				isAdded = true;
			}

			return isAdded;
		}


		public bool AddPackageToPallet(Order order, List<OrderLine> ordLines, List<Slotting> slottings, 
			double buildToPct, double overflow, bool stable, bool enforceStability, int maxUnstablePkgs, double maxUnstablePct)
		{
			bool isAdded = false;

			if (CanAdd(ordLines, buildToPct, overflow, enforceStability, maxUnstablePkgs, maxUnstablePct))
			{
				isAdded = true;

				foreach (OrderLine orderLine in ordLines)
				{
					Slotting slotInfo = slottings.FindFirst(orderLine.Sku.InventoryId);
					
					isAdded = this.AddItemToPallet(order, orderLine, orderLine.CaseQuantityRemaining, 
						slotInfo, buildToPct, overflow, stable, enforceStability, maxUnstablePkgs, maxUnstablePct);
				}
			}

			return isAdded;
		}

		public bool addPackagetoPalletWithoutCheck(Order order, List<OrderLine> ordLines, List<Slotting> slottings)
		{
			foreach (OrderLine orderLine in ordLines)
			{
				Slotting slotting = slottings.FindFirst(orderLine.Sku.InventoryId);
					
				AddItemToPalletWithoutCheck(order, orderLine, orderLine.CaseQuantityRemaining, slotting);
			}

			return true;
		}

		public int GetPkgPriorityDiff(Package package)
		{
			int lowestDifference = 99999999;

			foreach (PalletItem palletItem in PalletItems)
			{
				int packagePriorityDifference = Math.Abs(package.Priority - palletItem.Sku.Package.Priority);

				if (packagePriorityDifference < lowestDifference)
				{
					lowestDifference = packagePriorityDifference;
				}
			}

			return lowestDifference;
		}

		public bool ContainsPackage(int packageId) => PalletItems.Any(pi => pi.Sku.Package.PackageId == packageId);


		public bool CanAdd(OrderLine orderLine, int casesQuantity, double buildToPercentage, double overflow, bool enforceStability, 
			int maxUnstablePackageCount, double maxUnstablePercentage)
		{
			bool canBeAdded = false;
			bool stabilityOK = true;

			double percentageOfOrderLine = orderLine.CalculatePercentageSkuOfItself(casesQuantity);
			
			bool spaceOK = CheckSpace(
				percentageOfOrderLine + GetPercentageFull(), 
				buildToPercentage + overflow);
			
			if (enforceStability)
			{
				int unstablePackageCountAdd = 0;

				if (!UnstablePackageIds.Contains(orderLine.Sku.Package.PackageId))
				{
					unstablePackageCountAdd = 1;
				}

				if (UnstablePackageIds.Count + unstablePackageCountAdd > maxUnstablePackageCount)
				{
					if (percentageOfOrderLine + UnstablePercentage <= maxUnstablePercentage)
					{
						stabilityOK = true;
					}
					else
					{
						stabilityOK = false;
					}
				}
			}

			if (spaceOK && stabilityOK)
			{
				canBeAdded = true;
			}

			if (!canBeAdded)
			{
				if (!stabilityOK)
				{
					// '               logger.debug(("Stability rules violated.  Cannot add " _
					// '                               + (orderLine.getSku.getInvenId + ("(" _
					// '                               + (orderLine.getSku.getPkg.getPkgId + (") to pallet " _
					// '                               + (Me.palNum + (" " _
					// '                               + ((pctOfOrderLine + pctUnstable) + (" > " + maxUnstablePct))))))))))
				}
				else if (!spaceOK)
				{
					// '              logger.debug(("Full pallet max exceeded.  Cannot add " _
					// '                              + (orderLine.getSku.getInvenId + ("(" _
					// '                              + (orderLine.getSku.getPkg.getPkgId + (") to pallet " _
					// '                              + (Me.palNum + (" " _
					// '                              + ((pctOfOrderLine + getPctFull()) + (" > " _
					// '                              + (buildToPct + overflow)))))))))))
				}
			}

			return canBeAdded;
		}

		public bool CanAdd(List<OrderLine> orderLines, double buildToPercentage, double overflow, bool enforceStability, 
			int maxUnstablePackages, double maxUnstablePercentage)
		{
			bool isAddOk = false;

			if (orderLines.Count > 0)
			{
				double pctOfOrderLines = 0;

				foreach (OrderLine ol in orderLines)
				{
					pctOfOrderLines += ol.CalculatePercentageSkuOfItself();
				}

				bool isSpaceOk = CheckSpace(pctOfOrderLines + GetPercentageFull(), buildToPercentage + overflow);
				bool isStabilityOk = true;
				
				if (enforceStability)
				{
					int unstablePkgCountAdd = 0;

					if (!UnstablePackageIds.Contains(orderLines[0].Sku.Package.PackageId))
					{
						unstablePkgCountAdd = 1;
					}

					if (UnstablePackageIds.Count + unstablePkgCountAdd > maxUnstablePackages)
					{
						isStabilityOk = pctOfOrderLines + UnstablePercentage <= maxUnstablePercentage;
					}
				}

				if (isSpaceOk && isStabilityOk)
				{
					isAddOk = true;
				}

				if (!isAddOk)
				{
					if (!isStabilityOk)
					{
						// '             logger.debug(("Stability rules violated.  Cannot add to pallet " _
						// '                             + (Me.palNum + (" " _
						// '                             + ((pctOfOrderLines + Me.pctUnstable) + (" > " + maxUnstablePct))))))
					}
					else if (!isSpaceOk)
					{
						// '              logger.debug(("Full pallet max exceeded.  Cannot add to pallet " _
						// '                              + (Me.palNum + (" " _
						// '                              + ((pctOfOrderLines + getPctFull()) + (" > " _
						// '                              + (buildToPct + overflow)))))))
					}
				}
			}

			return isAddOk;
		}


		private bool CheckSpace(double buildPercentage, double buildToPercentage)
		{
			bool isSpaceOk = buildPercentage <= buildToPercentage;

			if (!isSpaceOk)
			{
				double exceededBy = buildPercentage - buildToPercentage;

				if (exceededBy < 0.00000001)
				{
					isSpaceOk = true;
				}
			}

			return isSpaceOk;
		}

	}
}
