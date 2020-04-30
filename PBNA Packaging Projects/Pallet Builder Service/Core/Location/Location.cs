using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Location
	{
		public int LocationId { get; set; }

		public string Name { get; set; }

		public int OrderCycleHoursBulk { get; set; }
		public int OrderCycleHoursDBay { get; set; }


		public DateTime DBayCutoff { get; set; }
		public DateTime BulkCutoff { get; set; }

		public string BulkRouteFlag { get; set; }
		public string DBayRouteFlag { get; set; }


		public int BulkThreshold { get; set; }
		public int DBayThreshold { get; set; }

		public List<Rule> Rules { get; set; }

		public double FullPalletMaxPct { get; set; }

		public double MaximumOverflow { get; set; }
		public double MaximumUnstablePct { get; set; }
		public int MaximumUnstableQuantity { get; set; }

		public int StandardPalletId { get; set; }

		public double FullBayMaximumPct { get; set; }

		public int CO2TankWeight20lb { get; set; }

		public int DBayLoadExtractSingleSkuFullPallets { get; set; }
		public int DBayLoadExtractSingleSkuFullBayPallets { get; set; }
		public int DBayLoadExtractSplitGroups { get; set; }

		public List<BuildMethod> BuildMethods { get; set; }

		public int PrimaryBuild { get; set; }

		public bool ApbFlag { get; set; }
		public bool AbmFlag { get; set; }

		public List<Slotting> Slottings { get; set; }

		public Item DefaultPallet { get; set; } = new Item();

		public int MinimumLayerAtmn { get; set; }

		public double DBayTruckMinimumFillPct { get; set; }

		public double TallBottleHeight { get; set; }

		public double AbmMinimumLayerHighSku { get; set; }
		public double AbmMinimumLayerLowSku { get; set; }
		public double AbmHeavyItemMinimumHeight { get; set; }

		public double TopNSkuPercent { get; set; }
		public double OrderBasePercent { get; set; }

		public bool HasUcv { get; set; }

		public double OrderCountWeightage { get; set; }
		public double SkuCountWeightage { get; set; }
		public double TotalItemWeightWeightage { get; set; }

		public string DefaultNisPalletType { get; set; }

		public bool SapOrderFlag { get; set; }

		// --- MERCH'18

		public string MerchandisePalletEnableFlag { get; set; }

		// --- GeoBox Enhancements '19

		public bool TransportSeparateByRouteEnableFlag { get; set; }
		public int TransportSequenceByRouteSortOrder { get; set; }

		public bool RouteBasedDeliveryEnableFlag { get; set; }

		

		private double DefaultPalletHeight 
		{ 
			set { DefaultPallet.Height = value; } 
		}
		private double DefaultPalletWidth
		{
			set { DefaultPallet.Width = value; }
		}
		private double DefaultPalletLength
		{
			set { DefaultPallet.Length = value; }
		}
	}
}
