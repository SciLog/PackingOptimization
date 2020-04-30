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
			try
			{
				// --- Location ---

				Location location = new LocationRepository().GetLocationInfo(505, new DateTime(2019, 6, 10));

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"{location.FullBayMaximumPct}");
				Console.WriteLine();


				// --- Rules ---

				List<Rule> rules = new RuleRepository().GetRules(505, "009");

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"{rules.Count} Rules Returned:");
				Console.WriteLine();
				
				foreach(Rule rule in rules)
				{
					Console.WriteLine($"{rule.Code}, {rule.IsActive}, {rule.OrderType}");
				}


				// --- Order Sourcing Matrix

				List<OrderSourcingMatrix> orderSourcingMatrices = new OrderSourcingMatrixRepository().GetOrderSourcingInfo(
					505,
					DeliveryMethodCode.Bulk,
					new DateTime(2019, 6, 10));

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"{orderSourcingMatrices.Count} Order Sourcing Matrix Returned:");
				Console.WriteLine();
				
				foreach (OrderSourcingMatrix orderSourcingMatrix in orderSourcingMatrices)
				{
					Console.WriteLine($"{orderSourcingMatrix.BuildLocationId}, {orderSourcingMatrix.Id}, {orderSourcingMatrix.SellingLocationId}, {orderSourcingMatrix.PickMethodCode}");
				}


				// Build Methods

				List<BuildMethod> buildMethods = new BuildMethodRepository().GetBuildMethodsForLocation(
					new DateTime(2019, 6, 10),
					orderSourcingMatrices);

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"{buildMethods.Count} Build Methods Returned:");
				Console.WriteLine();

				foreach (BuildMethod buildMethod in buildMethods)
				{
					Console.WriteLine($"{buildMethod.LocationId}, {buildMethod.BuildMethodCode}, {buildMethod.PickMethodCode}, {buildMethod.MinimumNumberOfLayers}");
				}


				// Slotting

				//List<Slotting> slottings = new SlottingRepository().GetSlottingsForLocation(
				//	505,
				//	new DateTime(2019, 6, 10));

				List<Slotting> slottings = new SlottingRepository().GetSlottingsForLocation(
					buildMethods,
					DeliveryMethodCode.Bulk);

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"{slottings.Count} Slottings Returned:");
				Console.WriteLine();

				foreach (Slotting slotting in slottings.Where(s => s.Cell > 0))
				{
					Console.WriteLine($"{slotting.BuildLocationId}, {slotting.PickDirectionCode}, {slotting.PickMethodCode}, {slotting.InventoryId}");
				}


				// Unstackables

				List<Unstackable> unstackables = new UnstackableRepository().GetForLocation(260);

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"{unstackables.Count} Unstackables Returned:");
				Console.WriteLine();

				foreach (Unstackable unstackable in unstackables)
				{
					Console.WriteLine($"{unstackable.TopPackageId}, {unstackable.BottomPackageId}, {unstackable.PackageIdToRemove}");
				}


				// Unstackables

				Item defaultItemConfig = new ItemRepository().GetDefaultConfig(1);

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine($"Default Item Config: {defaultItemConfig.Height}, {defaultItemConfig.Width}, {defaultItemConfig.Weight}");
				Console.WriteLine();

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			return Task.CompletedTask;
		}

    }
}
