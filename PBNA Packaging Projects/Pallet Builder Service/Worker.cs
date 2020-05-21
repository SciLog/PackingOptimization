using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScientificLogistics.PalletBuilder.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace ScientificLogistics.PalletBuilder
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;

		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//int locationId = 248;
			int locationId = 251;

			try
			{
				// -- Load Orders --

				Console.WriteLine();
				Console.WriteLine("----------> Loading Orders <----------");
				Console.WriteLine();

				List<Order> orders = new OrderRepository().GetPrioritizedWorkUnitsForLoc(locationId);

				Console.WriteLine($"{orders.Count} Orders Returned for Location ID = {locationId}");
				Console.WriteLine();

				Random random = new Random();

				foreach (Order o in orders)
				{
					Console.WriteLine($"\t --- Order Id = {o.OrderId}, Location Id = {o.Location.LocationId}, " +
						$"Build Date = {o.BuildDate:d}, Std Pallet Only = {o.IsStandardPalletOnly}, " +
						$"Customer Id = {o.Customer.CustomerId}");

					foreach (OrderLine orderLine in o.OrderLines)
					{
						orderLine.CustomerOrderableUnits = random.Next(5, 500);

						Console.WriteLine($"\t\t *** ol id = {orderLine.OrderLineId}, COU = {orderLine.CustomerOrderableUnits}, Inv ID = {orderLine.Sku.InventoryId}, Pkg Id = {orderLine.Sku.Package.PackageId}");

						//Console.WriteLine($"\t\t *** ol id = {orderLine.OrderLineId}, case qty = {orderLine.CaseQuantity}, remaining = {orderLine.CaseQuantityRemaining}, " +
						//	$"itm brd id = {orderLine.Sku.Brand.BrandId}, itm pkg id = {orderLine.Sku.Package.PackageId}, " +
						//	$"itm pkg full plt q = {orderLine.Sku.Package.FullPalletQuantity}, itm pkg c per lyr = {orderLine.Sku.Package.CasesPerLayer}, " +
						//	$"itm lyr / plt = {orderLine.Sku.Package.LayersPerPallet}, inv id = {orderLine.Sku.InventoryId}, " +
						//	$"itm fp f plt qty = {orderLine.Sku.FpFullPalletQuantity}, itm fp c / lyr = {orderLine.Sku.FpCasesPerLayer}, " +
						//	$"itm fp lyr / plt = {orderLine.Sku.FpLayersPerPallet}, itm plt type cd = {orderLine.Sku.PalletTypeCode}, " +
						//	$"itm chilled = {orderLine.Sku.IsChilled}");
					}

					Console.WriteLine();
				}

				Order order = orders.Skip(0).First();


				// -- Load Rules --

				Console.WriteLine();
				Console.WriteLine("----------> Loading Rules <----------");
				Console.WriteLine();


				List<Rule> rules = new RuleRepository().GetRulesForLocation(order.Location.LocationId);

				Console.WriteLine($"{rules.Count} Rules Returned for Order ID = {order.OrderId} @ Location ID = {order.Location.LocationId}");
				Console.WriteLine();

				
				// -- Load Slottings --

				//Console.WriteLine();
				//Console.WriteLine("----------> Loading Slottings <----------");
				//Console.WriteLine();


				//List<Slotting> slottings = new SlottingRepository().GetSlottingsForLocation(
				//	bulkOrder.Location.LocationId, 
				//	bulkOrder.DeliveryDate.Date);

				//Console.WriteLine($"\t{slottings.Count} Slottings Returned for Orderi ID = {bulkOrder.OrderId} ON {bulkOrder.DeliveryDate:d}");
				//Console.WriteLine();


				// -- Palletize! --

				Console.WriteLine();
				Console.WriteLine("----------> Palletizing <----------");
				Console.WriteLine();

				Palletizer palletizer = new Palletizer();
				
				List<Pallet> pallets = palletizer.palletize(order, rules, false);

				Console.WriteLine();
				Console.WriteLine($"{pallets.Count} Pallets prepared");
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("********************");
				Console.WriteLine();
				Console.WriteLine(ex.Message);
				Console.WriteLine();
				Console.WriteLine("********************");
				Console.WriteLine();
			}

			return Task.CompletedTask;
		}


		//protected override Task ExecuteAsync(CancellationToken stoppingToken)
		//{
		//	try
		//	{
		//		// --- Location ---

		//		Location location = new LocationRepository().GetLocationInfo(505, new DateTime(2019, 6, 10));

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{location.FullBayMaximumPct}");
		//		Console.WriteLine();


		//		// --- Rules ---

		//		List<Rule> rules = new RuleRepository().GetRules(505, "009");

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{rules.Count} Rules Returned:");
		//		Console.WriteLine();

		//		foreach(Rule rule in rules)
		//		{
		//			Console.WriteLine($"{rule.Code}, {rule.IsActive}, {rule.OrderType}");
		//		}


		//		// --- Order Sourcing Matrix

		//		List<OrderSourcingMatrix> orderSourcingMatrices = new OrderSourcingMatrixRepository().GetOrderSourcingInfo(
		//			505,
		//			DeliveryMethodCode.Bulk,
		//			new DateTime(2019, 6, 10));

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{orderSourcingMatrices.Count} Order Sourcing Matrix Returned:");
		//		Console.WriteLine();

		//		foreach (OrderSourcingMatrix orderSourcingMatrix in orderSourcingMatrices)
		//		{
		//			Console.WriteLine($"{orderSourcingMatrix.BuildLocationId}, {orderSourcingMatrix.Id}, {orderSourcingMatrix.SellingLocationId}, {orderSourcingMatrix.PickMethodCode}");
		//		}


		//		// Build Methods

		//		List<BuildMethod> buildMethods = new BuildMethodRepository().GetBuildMethodsForLocation(
		//			new DateTime(2019, 6, 10),
		//			orderSourcingMatrices);

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{buildMethods.Count} Build Methods Returned:");
		//		Console.WriteLine();

		//		foreach (BuildMethod buildMethod in buildMethods)
		//		{
		//			Console.WriteLine($"{buildMethod.LocationId}, {buildMethod.BuildMethodCode}, {buildMethod.PickMethodCode}, {buildMethod.MinimumNumberOfLayers}");
		//		}


		//		// Slotting

		//		//List<Slotting> slottings = new SlottingRepository().GetSlottingsForLocation(
		//		//	505,
		//		//	new DateTime(2019, 6, 10));

		//		List<Slotting> slottings = new SlottingRepository().GetSlottingsForLocation(
		//			buildMethods,
		//			DeliveryMethodCode.Bulk);

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{slottings.Count} Slottings Returned:");
		//		Console.WriteLine();

		//		foreach (Slotting slotting in slottings.Where(s => s.Cell > 0))
		//		{
		//			Console.WriteLine($"{slotting.BuildLocationId}, {slotting.PickDirectionCode}, {slotting.PickMethodCode}, {slotting.InventoryId}");
		//		}


		//		// Unstackables

		//		List<Unstackable> unstackables = new UnstackableRepository().GetForLocation(260);

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{unstackables.Count} Unstackables Returned:");
		//		Console.WriteLine();

		//		foreach (Unstackable unstackable in unstackables)
		//		{
		//			Console.WriteLine($"{unstackable.TopPackageId}, {unstackable.BottomPackageId}, {unstackable.PackageIdToRemove}");
		//		}


		//		// Default Item Config

		//		Item defaultItemConfig = new ItemRepository().GetDefaultConfig(1);

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"Default Item Config: {defaultItemConfig.Height}, {defaultItemConfig.Width}, {defaultItemConfig.Weight}");
		//		Console.WriteLine();


		//		// Order

		//		List<Order> orders = new OrderRepository().GetOrdersForLocation(249);

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{orders.Count} Orders Returned:");
		//		Console.WriteLine();

		//		foreach (Order order in orders)
		//		{
		//			Console.WriteLine($"{order.OrderId}, {order.DeliveryLocation.LocationId}, {order.OrderTypeCode}");
		//		}


		//		// Order Lines

		//		List<OrderLine> orderLines = new OrderLineRepository().GetOrderLines(486310816, PalletOrderStatus.WAITING);

		//		Console.WriteLine();
		//		Console.WriteLine();
		//		Console.WriteLine($"{orderLines.Count} Order Lines Returned:");
		//		Console.WriteLine();

		//		foreach (OrderLine orderLine in orderLines)
		//		{
		//			Console.WriteLine($"{orderLine.OrderId}, {orderLine.CaseQuantity}, {orderLine.PalletCaseQuantity}");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.WriteLine(ex.Message);
		//	}

		//	return Task.CompletedTask;
		//}

	}
}
