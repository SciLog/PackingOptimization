using ScientificLogistics.PalletBuilder.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public class Palletizer
	{
		#region -- Constants --

		private string[] STARTER_PALLET_TYPE_CODES =
		{
			"004", "005", "006", "007", "008", "009", "011",
			"012", "013", "014", "015", "016", "024", "025", 
			"026", "027", "028", "029", "030", "031", "035", 
			"036", "037", "039", "045"
		};

		#endregion


		#region -- Public Methods --

		public List<Pallet> palletize(Order order, List<Rule> rules, bool validatePallets)
		{
			List<Pallet> pallets = new List<Pallet>();						// Output Pallets
			List<OrderLine> originalOrderLines = order.OrderLines;		// Used To Restore After Palletization


			// Create pallets for each item group

			Dictionary<string, List<OrderLine>> orderLineDictionary = order.GetOrderLinesByType(rules);		// Group By Order Line Type

			foreach (string orderLineTypeCode in orderLineDictionary.Keys)
			{
				if (orderLineTypeCode != OrderLineTypeCode.All && orderLineDictionary[orderLineTypeCode].Count > 0)
				{
					order.OrderLines = orderLineDictionary[orderLineTypeCode];      // Temporarily Overwrite Order Lines For Palletizing	

					pallets.AddRange(                                                   // Palletize 
						PalletizeBulkOrder(
							order,
							rules,
							validatePallets));
				}
			}


			// Restore Full Order Lines (was hijacked above)

			order.OrderLines = originalOrderLines;


			// Sequence Pallets

			pallets.Resequence();


			return pallets;
		}

		#endregion


		#region -- Private Methods --

		private List<Pallet> PalletizeBulkOrder(Order order, List<Rule> rules, bool validatePallets)
		{
			LocationRepository locationRepository = new LocationRepository();

			// Geobox:  reset the default notification to mitigate layer items being dropped from Slotting for standard pallet picks and 
			// avoid using notifications generated from half pallet palletization

			order.DefaultNotification = null;


			// logger.debug(("Gathering location and item information for containerization of order " + bulkOrder.getOrdId))

			order.IsPalletizerRun = true;


			// Check the Pick Methods.  Make sure there is one for Full Pallets and one for Eaches

			//CheckPickMethods(bulkOrder);


			// Get location information (full pallet max, max overflow, etc)

			Console.WriteLine();
			Console.WriteLine("----------> Loading Sales Location Info <----------");
			Console.WriteLine();

			Location salesLocation = locationRepository.GetLocationInfo(
				order.Location.LocationId,
				order.BuildDate);

			CheckAndCombineLocationInformation(
				order.Location, 
				salesLocation, 
				order);


			// Get primary build location if it's null.  Miinimum layer requirement has to be read. 
			// hence call getLocationProfile unconditionally

			Console.WriteLine();
			Console.WriteLine("----------> Loading Build Location Profile <----------");
			Console.WriteLine();

			order.PrimaryBuildLocation = GetPrimaryBuildsLocationProfile(order);

			Location primaryBuildLocationInfo = locationRepository.GetLocationInfo(
				order.PrimaryBuildLocation.LocationId,
				order.BuildDate);

			CheckAndCombineLocationInformation(
				order.PrimaryBuildLocation,
				primaryBuildLocationInfo,
				order);


			// Get the build methods and associated pick methods for this location

			Console.WriteLine();
			Console.WriteLine("----------> Loading Order Sourcing Matrices <----------");
			Console.WriteLine();

			OrderSourcingMatrixRepository orderSourcingMatrixRepository = new OrderSourcingMatrixRepository();

			int locationId = order.IsBuildIdOverriden ? 
				order.PrimaryBuildLocation.LocationId : 
				order.Location.LocationId;

			List<OrderSourcingMatrix> orderSourcingMatrices = orderSourcingMatrixRepository.GetOrderSourcingInfo(
				locationId,
				order.DeliveryMethodCode,
				order.BuildDate);

			foreach (OrderSourcingMatrix orderSourcingMatrix in orderSourcingMatrices)
			{
				Console.WriteLine("\t - Build Location = " + orderSourcingMatrix.BuildLocationId
					+ ", pick method = " + orderSourcingMatrix.PickMethodCode
					+ ", build Date = " + order.BuildDate);
			}

			order.OrderSourcingMatrices = orderSourcingMatrices;


			// -----

			Console.WriteLine();
			Console.WriteLine("----------> Loading Building Methods <----------");
			Console.WriteLine();

			BuildMethodRepository buildMethodRepository = new BuildMethodRepository();

			List<BuildMethod> buildMethods = buildMethodRepository.GetBuildMethodsForLocation(
				order.BuildDate,
				order.OrderSourcingMatrices);

			order.PrimaryBuildLocation.BuildMethods = buildMethods;

			// -----

			string deliveryMethodCode = DeliveryMethodCode.Bulk;
			
			if(rules.IsRuleActive(RuleTypeCode.ProcessHalfAndStandardPallet, order))
			{
				if (!order.IsStandardPalletOnly)
				{
					deliveryMethodCode = DeliveryMethodCode.Bay;
				}
			}


			// Get the slotting information for each build method
			// Also check the order to make sure all items are slotted in Eaches	

			Console.WriteLine();
			Console.WriteLine("----------> Loading Slotting <----------");
			Console.WriteLine();

			List<Slotting> slottings = new SlottingRepository()
				.GetSlottingsForLocation(buildMethods, deliveryMethodCode);

			CheckOrderAgainstSlotting(
				order,
				slottings,
				buildMethods.GetBuildLocationIdForPickMethod(PickMethodCode.Eaches),
				buildMethods.GetBuildLocationIdForPickMethod(PickMethodCode.Chilled));


			/// Dim orderType As String = "Bulk"

			var pallets = new List<Pallet>();


			// Get the item configurations for this location.
			// Combine the information with the respective items on the order

			Console.WriteLine();
			Console.WriteLine("----------> Loading Item Info <----------");
			Console.WriteLine();

			CheckAndCombineItemInfo(
				order.PrimaryBuildLocation.LocationId,
				true,
				order,
				rules);


			// Convert customer orderable units to cases if delivery method is bulk
			// Conversion has already been done for BOL orders

			if (order.DeliveryMethodCode == DeliveryMethodCode.Bulk)
			{
				// if palletize merch, conversion has already take place, so do not convert again
				// assuming bulkOrder.getLocation() will always return the sales location
				// the merch rule should always be checked at the sales location
				
				if(order.Location.IsMerchandisePalletEnable || order.Customer.PalletizationOption != PalletizationOptionCode.RuleBased)
				{
					order.ConvertCustomerOrderableUnitstoCases(true);

					// Combine quantities of order lines with the same SKU
					// Only needed if delivery method is BULK.  Same process already done for BOL

					order.CombineSameSkuOrderLines();
				}
			}


			// For logging information to debug the infinite loop problem
			// checkPalletConfigurations(bulkOrder)


			// logger.debug(("Beginning containerization of " + (orderType + (" order " + bulkOrder.getOrdId))))

			if (order != null)
			{
				slottings.removeSlottingForDefaultItems(order.DefaultNotification);
			}

			if (!order.IsStandardPalletOnly)
			{
				// Force only Eaches slotting for half pallet processing
				slottings.RemoveAutomationSlottingForItems(order.OrderLines);
			}


			// ----------------------------------
			// (1) Create Single SKU Full Pallets
			// ----------------------------------
			// If (bulkOrder.isStandardPalletOnly AndAlso Not bulkOrder.hasChilled) Then

			// count = createFullPallets(bulkOrder, pallets)

			// ElseIf (rulesDelegate.isRuleActive(Constants.RULE_ID_SPRINT_BIB, rules, bulkOrder.Location.LocId, bulkOrder.ordTypeCde, bulkOrder.getDmCde) AndAlso bulkOrder.hasBIB) Then

			Console.WriteLine();
			Console.WriteLine("----------> Creating Pallets From Order Lines <----------");
			Console.WriteLine();
			
			int count = CreateFullPallets(order, pallets);

			// End If


			// -------------------------------------------------
			// (2) Create Single Package Full Layer Full Pallets
			// -------------------------------------------------
			// count = createSinglePkgFullLayerPallets(bulkOrder, pallets, Slotting, 1, 0)



			// ------------------------------------------------
			// (3) Create Multi Package Full Layer Full Pallets
			// ------------------------------------------------
			// count = createMultiPkgFullLayerPallets(bulkOrder, pallets, Slotting)



			// ----------------------------------
			// (4) Create Single Package  Pallets
			// ----------------------------------
			// makeSinglePackagePallets(bulkOrder, pallets, slotting, bulkOrder.getPrimaryBuild.getFullPalMaxPct, bulkOrder.getPrimaryBuild.getMaxOverflow)


			// ---------------------------------------------------
			// (5) Create Multi Package Full Layer Partial Pallets
			// ---------------------------------------------------
			// Dim fullLayerPallets As List(Of Pallet) = createMultiPkgMultiCellFullLayerPallets(bulkOrder, slotting, 1)


			// --------------------------------------------------
			// (6) Create Single Package Mixed Layer Full Pallets
			// --------------------------------------------------
			// Dim pickConversion As Boolean = False
			// Dim mergedPallets As List(Of Pallet) = New List(Of Pallet)
			// Dim mixedLayerPartialPallets As List(Of Pallet) = New List(Of Pallet)
			// Dim mergeDonePallets As List(Of Pallet) = New List(Of Pallet)

			// Do Until pickConversion

			// count = createSinglePkgMixedLayerPallets(bulkOrder, pallets, mixedLayerPartialPallets, slotting, 1, 0, False)


			// ----------------------------------------------
			// (7) combine full layer and mixed layer pallets
			// ----------------------------------------------
			// pickConversion = combineFullLayerAndMixedLayerPallets(bulkOrder, slotting, fullLayerPallets, mixedLayerPartialPallets, pallets.Count, mergedPallets)

			// If pickConversion Then

			// mixedLayerPartialPallets.Clear()
			// fullLayerPallets.Clear()

			// For Each p As Pallet In mergedPallets

			// If p.isLayersRemoved Then
			// mixedLayerPartialPallets.Add(p)
			// Else
			// mergeDonePallets.Add(p)
			// End If

			// Next
			// End If


			// Loop


			// mergedPallets.AddRange(mergeDonePallets)


			// ----------------------------------------------
			// (8) Create Single Package Eaches Full Pallets
			// ----------------------------------------------

			// For Each p As Pallet In mergedPallets
			// removeUnStackableLayers(p, bulkOrder)
			// Next
			// For Each p As Pallet In pallets
			// removeUnStackableLayers(p, bulkOrder)
			// Next

			// count = createSinglePkgEachesPallets(bulkOrder, pallets, slotting, 1, 0)


			// ------------------------
			// (9) Create Build Pallets
			// ------------------------
			// count = createBuildPallets(bulkOrder, pallets, mergedPallets, slotting)


			// ----------------------------------------------
			// (10) Balance the full layer work between cells
			// ----------------------------------------------
			// If RulesDelegate.isRuleActive(bulkOrder, Constants.RULE_CDE_LOAD_BALANCE_HSLP, rules) Then
			// Dim fullLayerSlotting As List(Of Slotting) = bulkContainerizeDelegate.getByPickMethod(slotting, Constants.PICK_MTHD_CDE_FULL_LAYER)
			// loadBalanceFullLayers(bulkOrder.getOrdId, pallets, fullLayerSlotting)
			// End If


			// Dim totalRemaining As Integer = 0
			// For Each ol As OrderLine In bulkOrder.getOrdLines
			// totalRemaining = (totalRemaining + ol.getCaseQtyRemaining)
			// Next


			// For Each p As Pallet In pallets

			// p.orderLayers(getLayerComparator)

			// If Not bulkOrder.isStandardPalletOnly Then
			// p.setNisPalTypeCde(Constants.NIS_HALF_PALLET_CDE)
			// End If

			// Next p


			// Check parameter validatePallets to determine whether to validate order pallets or not
			if (validatePallets)
			{
				// checkPalletsAndOrder(bulkOrder, pallets)
				// validatePalletData(pallets)
			}

			return pallets;
		}

		private Dictionary<string, List<OrderLine>> CheckAndCombineItemInfo(int locationId, bool initialUseDefaultPallet, Order order, List<Rule> rules)
		{
			bool isStandardPalletOnly = order.IsStandardPalletOnly;
			bool useDefaultPallet = initialUseDefaultPallet;

			Item defaultItem = null;

			List<OrderLine> orderLines = order.OrderLines;
			List<OrderLine> invalidInvens = new List<OrderLine>();
			Dictionary<string, List<OrderLine>> errInvenMap = new Dictionary<string, List<OrderLine>>();


			// -- Default Items --

			ItemRepository itemRepository = new ItemRepository();

			Item reg_defaultItem = itemRepository.GetDefaultConfig(1);
			Item halfDefaultItem = itemRepository.GetDefaultConfig(1);
			Item chilledDefaultItem = itemRepository.GetDefaultConfig(1);


			// -- Chilled Defaults --

			Package chill_pkg = chilledDefaultItem.Package;

			chill_pkg.CasesPerLayer = ChilledDefault.CasesPerLayer;
			chill_pkg.LayersPerPallet = ChilledDefault.LayersPerPallet;
			chill_pkg.FullPalletQuantity = ChilledDefault.CasesPerPallet;


			// -- Half Pallet Defaults --

			Package half_pkg = halfDefaultItem.Package;

			half_pkg.CasesPerLayer /= 2;
			half_pkg.FullPalletQuantity /= 2;


			// -- Active Rules --

			bool ruleBIB = rules.IsRuleActive(RuleTypeCode.SprintBib, order);
			bool ruleCO2 = rules.IsRuleActive(RuleTypeCode.CO2Aggregation, order);


			// -- Location Items

			List<Item> locItems = itemRepository.GetItemsForLocation(locationId);


			//-------------------------------------------------------------------------------
			// KEY ERROR CHECK - ARE ALL SKUS ON THE ORDER ARE AVAILABLE AT THE WAREHOUSE
			// SETTING PALLET TYPE CODE DEFAULTS
			//------------------------------------------------------------------------------
			// Make sure all invens on the order have a corresponding loc inven record

			foreach (OrderLine orderLine in orderLines)
			{
				Item sku = orderLine.Sku;
				int key = sku.InventoryId;

				if (orderLine.Sku.IsChilled)
				{
					defaultItem = chilledDefaultItem;
					orderLine.Sku.PalletTypeCode = PalletTypeCode.ChilledNIS;
					useDefaultPallet = false;
				}
				else if (ruleCO2 && orderLine.Sku.IsCO2Supply)
				{
					defaultItem = reg_defaultItem;
					orderLine.Sku.PalletTypeCode = PalletTypeCode.CO2NIS;
					useDefaultPallet = false;
				}
				else if (ruleBIB && orderLine.Sku.HasBib)   // If Rule is ON and the line item contains BIB item, than use the default item 
				{
					defaultItem = reg_defaultItem;
					orderLine.Sku.PalletTypeCode = PalletTypeCode.StandardNIS;
					useDefaultPallet = false;
				}
				else if (!isStandardPalletOnly)
				{
					defaultItem = halfDefaultItem;
					orderLine.Sku.PalletTypeCode = PalletTypeCode.HalfNIS;
					useDefaultPallet = false;
				}
				else
				{
					defaultItem = reg_defaultItem;
					orderLine.Sku.PalletTypeCode = null;
					useDefaultPallet = initialUseDefaultPallet;
				}


				defaultItem.InventoryId = key;

				List<Item> locItemsForInven = locItems
					.Where(i => i.InventoryId == key)
					.ToList();

				Item locItemInfo = null;


				foreach (Item locItem in locItemsForInven)
				{
					if (ruleBIB && orderLine.Sku.HasBib)
					{
						defaultItem.CustomerOrderableUintsPerCase = locItem.CustomerOrderableUintsPerCase;
					}

					if (
						((useDefaultPallet || sku.PalletTypeCode is null) && locItem.IsDefaultPallet) ||                    // Using default pallet or the pallet type code is null and this item is the default pallet
						(!useDefaultPallet && sku.PalletTypeCode != null && locItem.PalletTypeCode == sku.PalletTypeCode))  // OR not using default pallet and pal type code is not null and equals the loc item pal type code
					{
						locItemInfo = locItem;
						break;
					}
				}


				// -- Validation / Errors --

				//StringBuilder errCd = new StringBuilder();
				//locItemInfo = validateItemAttributes(locItemInfo, defaultItem, errCd);

				//if (errCd.length() > 0)
				//{
				//	invalidInvens = errInvenMap.get(errCd.toString());
				//	if (invalidInvens == null)
				//	{
				//		invalidInvens = new ArrayList<OrderLine>();
				//	}
				//	invalidInvens.add(ol);
				//	errInvenMap.put(errCd.toString(), invalidInvens);
				//}


				sku.Combine(locItemInfo);
			}

			return errInvenMap;
		}

		private int CreateFullPallets(Order order, List<Pallet> pallets)
		{

			// logger.info(("Entering Create Full Pallets for order => " + order.getOrdId))

			List<OrderLine> orderLines = order.OrderLines;
			
			int palletCount = 0;
			int numPallets;

			foreach (OrderLine orderLine in orderLines)
			{
				Item sku = orderLine.Sku;

				numPallets = this.CalculateNumberOfFullPallets(
					orderLine.CaseQuantityRemaining, 
					sku.FpFullPalletQuantity);
				
				int i = 0;
				
				while (i < numPallets)
				{
					Pallet pallet = new Pallet(order.PrimaryBuildLocation.DefaultPallet)
					{
						PalletNumber = pallets.Count + 1,
						PalletType = PalletType.SingleSkuFullPallet
					};


					// --------------------------------
					// Create slotting object on the fly
					// --------------------------------
					var fullPalletSlotting = new Slotting();
					var fullPalletOrderSourcingMatrix = new OrderSourcingMatrix();

					foreach (OrderSourcingMatrix orderSourcingMatrix in order.OrderSourcingMatrices)
					{
						if(orderSourcingMatrix.PickMethodCode == PickMethodCode.FullPallet)
						{
							fullPalletOrderSourcingMatrix = orderSourcingMatrix;
							break;
						}
					}

					fullPalletSlotting.BuildLocationId = fullPalletOrderSourcingMatrix.BuildLocationId;
					fullPalletSlotting.BuildDate = order.BuildDate;
					fullPalletSlotting.PickMethodCode = PickMethodCode.FullPallet;
					
					fullPalletSlotting.PickZoneCode = STARTER_PALLET_TYPE_CODES.Contains(sku?.PalletTypeCode ?? "") ?
						BuildMethodCode.STRT :
						BuildMethodCode.FULL;

					fullPalletSlotting.PickAreaCode = "---";

					// ---------------------------------
					// End of on-they-fly slotting object
					// ---------------------------------

					bool success = pallet.AddItemToPalletWithoutCheck(
						order, 
						orderLine, 
						sku.FpFullPalletQuantity, 
						fullPalletSlotting);
					
					if (success)
					{
						pallets.Add(pallet);
						orderLine.CaseQuantityRemaining -= sku.FpFullPalletQuantity;
						palletCount += 1;
					}

					i += 1;
				}

				Console.WriteLine($"\t - OrderLine {orderLine.OrderLineId} Produced {numPallets} Pallets " +
					$"({orderLine.Sku.FpFullPalletQuantity * numPallets} Cases Palletized, {orderLine.CaseQuantityRemaining} Cases Remain, Pkg ID = {orderLine.Sku.Package.PackageId}, " +
					$"Inv ID = {orderLine.Sku.InventoryId})");

			}

			return palletCount;
		}

		private int CalculateNumberOfFullPallets(int caseQuantity, int fullPalletQuantity)
		{
			return caseQuantity / fullPalletQuantity;
		}

		public void CheckAndCombineLocationInformation(Location masterLoc, Location comboLoc, Order order) 
		{
			if (comboLoc == null)
			{
				//throw new Exception("Location configuration(full pallet max, max overflow, etc) not set up for location " +
				//		masterLoc.LocationId + " and date " + order.BuildDate,
				//		Constants.BLK_CNTRZ_LOC_NOT_CONFIG,
				//		ExceptionType.APPLICATION,
				//		Severity.ERROR);
			}
			else
			{
				masterLoc.FullPalletMaxPct = (comboLoc.FullPalletMaxPct);
				masterLoc.MaximumOverflow = comboLoc.MaximumOverflow;
				masterLoc.MaximumUnstablePct = comboLoc.MaximumUnstablePct;
				masterLoc.MaximumUnstableQuantity = comboLoc.MaximumUnstableQuantity;
				masterLoc.IsApb = comboLoc.IsApb;
			}
		}

		public Location GetPrimaryBuildsLocationProfile(Order order)
		{
			LocationRepository locationRepository = new LocationRepository();
		
			//MH: ONLY get sales location profile if the build location has not been overwritten
			Location loc;

			if(!order.IsBuildIdOverriden)
			{
				loc = locationRepository.GetLocationProfile(
					order.Location.LocationId, 
					order.BuildDate); 

				return loc!=null ?
					locationRepository.GetLocationProfile(loc.PrimaryBuildLocationId, order.BuildDate) : 
					null; 
			}			
			else
			{
				return locationRepository.GetLocationProfile(
					order.PrimaryBuildLocation.LocationId, 
					order.BuildDate);
			}
			 		
	}

		public void CheckOrderAgainstSlotting(Order order, List<Slotting> slotting, int eachesLocationId, int chilledLocationId)
		{
			List<Slotting> eachesSlotting = slotting.FindAll(PickMethodCode.Eaches);
			List<Slotting> chilledSlotting = slotting.FindAll(PickMethodCode.Chilled);

			foreach (OrderLine orderLine in order.OrderLines)
			{
				if (orderLine.CustomerOrderableUnits > 0 && (orderLine.Sku.IsChilled ? chilledSlotting : eachesSlotting).FindFirst(orderLine.Sku.InventoryId) == null)
				{
					slotting.Add(
						orderLine.Sku.IsChilled ?
							Slotting.GetDefaultForChilled(chilledLocationId, orderLine.Sku.InventoryId, order) :
							Slotting.getDefaultForEaches(eachesLocationId, orderLine.Sku.InventoryId, order));
				}
			}
		}

		#endregion
	}
}
