using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Item
	{
		#region -- Public Properties --

		public string Name { get; set; }

		public Package Package { get; set; } = new Package();

		public Brand Brand { get; set; } = new Brand() { BrandId = 1 };

		public int InventoryId { get; set; }
		public int InventoryGroupId { get; set; } = 7;

		public double Height { get; set; } = 0;
		public double Width { get; set; } = 0;
		public double Length { get; set; } = 0;
		public double Weight { get; set; } = 0;

		public int FpCasesPerLayer { get; set; } = 0;
		public int FpLayersPerPallet { get; set; } = 0;
		public int FpFullPalletQuantity { get; set; } = int.MaxValue;

		public double CustomerOrderableUintsPerCase { get; set; } = 1;

		public string PalletTypeCode { get; set; }
		public string InventoryTypeCode { get; set; }

		public bool IsDefaultPallet { get; set; }
		public bool IsStarterPallet { get; set; }

		public int StackFactor { get; set; } = 0;

		public int PackageIdOverride { get; set; } = 0;

		public int BrandLayerSequence { get; set; } = 0;

		public int PalletItemNumber { get; set; } = 0;

		public string SupplyTypeCode { get; set; }
		public string ProductMixCode { get; set; }

		public bool IsChilled { get; set; }

		// ---

		public bool IsCO2Supply => SupplyTypeCode?.Equals(ItemTypeCode.C02Supply) ?? false;
		public bool IsPopSupply => SupplyTypeCode?.Equals(ItemTypeCode.POPSupply) ?? false;
		public bool HasBib => ProductMixCode == ItemTypeCode.BIBProductMix;

		#endregion

		#region -- Private Properties --

		// --- For Dapper SQL ---

		private int PackageId
		{
			set { Package.PackageId = value; }
		}
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
		private bool PackageIsUnstableInBay
		{
			set { Package.IsUnstableInBay = value; }
		}
		private int PackageDbayLayersPerPallet
		{
			set { Package.DbayLayersPerPallet = value; }
		}
		private string PackageContCode
		{
			set { Package.ContCode = value; }
		}
		private string PackagePackageContMtrlCode
		{
			set { Package.PackageContMtrlCode = value; }
		}

		private int BrandId
		{
			set { Brand.BrandId = value; }
		}
		private string BrandCategoryTypeCode
		{
			set { Brand.CategoryTypeCode = value; }
		}

		internal void Combine(Item item)
		{
			Package = item.Package;
			Height = item.Height;
			Width = item.Width;
			Length = item.Length;
			Weight = item.Weight;
			FpCasesPerLayer = item.FpCasesPerLayer;
			FpLayersPerPallet = item.FpLayersPerPallet;
			FpFullPalletQuantity = item.FpFullPalletQuantity;
			StackFactor = item.StackFactor;
			PackageIdOverride = item.PackageIdOverride;
			CustomerOrderableUintsPerCase = item.CustomerOrderableUintsPerCase;
			BrandLayerSequence = item.BrandLayerSequence;
			InventoryGroupId = item.InventoryGroupId;

			if (PalletTypeCode == null)
			{
				PalletTypeCode = item.PalletTypeCode;
			}

			Package.PackageContMtrlCode = item.Package.PackageContMtrlCode;
			Package.ContCode = item.Package.ContCode;

			Brand = item.Brand;
		}

		#endregion


		#region -- Public Methods --

		public Slotting GetSlotting(List<Slotting> slotting)
		{
			foreach (Slotting slot in slotting)
			{
				if (this.InventoryId == slot.InventoryId)
				{
					return slot;
				}
			}
			return null;
		}

		#endregion

	}
}
