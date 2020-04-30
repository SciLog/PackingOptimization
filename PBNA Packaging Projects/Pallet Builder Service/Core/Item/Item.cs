using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Item
	{
		public string Name { get; set; }

		public Package Package { get; set; } = new Package();

		public Brand Brand { get; set; }

		public int InventoryId { get; set; }
		public int InventoryGroupId { get; set; } = 7;

		public double Height { get; set; } = 0;
		public double Width { get; set; } = 0;
		public double Length { get; set; } = 0;
		public double Weight { get; set; } = 0;

		public int FpCasesPerLayer { get; set; } = 0;
		public int FpLayersPerPallet { get; set; } = 0;
		public int FpFullPalletQuantity { get; set; } = int.MaxValue;

		public double CostumerOrderDblUintsPerCase { get; set; } = 1;

		public string PalletTypeCode { get; set; }
		public string InventoryTypeCode { get; set; }

		public bool PrivIsDefaultPallet { get; set; }
		public bool PrivIsStarterPallet { get; set; }

		public int StackFactor { get; set; } = 0;

		public int PackageIdOverride { get; set; } = 0;

		public int BrandLayerSequence { get; set; } = 0;

		public int PalletItemNumber { get; set; } = 0;

		public string SupplyTypeCode { get; set; }
		public string ProductMixCode { get; set; }

		public bool IsChilled { get; set; }

		// --- For Dapper SQL ---

		private int PackageTypeId 
		{ 
			set { Package.PackageTypeId = value; }
		}
		private int PackagePriority
		{
			set { Package.Priority = value; }
		}
		private int PackageCasesPerLayer
		{
			set { Package.CasesPerLayer = value; }
		}
		private int PackageLayersPerPallet
		{
			set { Package.LayersPerPallet = value; }
		}
		private int PackageFullPalletQuantity
		{
			set { Package.FullPalletQuantity = value; }
		}

	}
}
