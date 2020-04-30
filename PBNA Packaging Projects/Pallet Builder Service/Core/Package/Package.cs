using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Package
	{
		public int PackageId { get; set; } = -1;
		public int PackageTypeId { get; set; }


		public int CasesPerLayer { get; set; }
		public int LayersPerPallet { get; set; }
		public int DbayLayersPerPallet { get; set; }
		public int FullPalletQuantity { get; set; }

		public int Priority { get; set; }

		public int MixedLayerPickSequence { get; set; }

		public bool IsUnstableInBay { get; set; }


		public string ContCode { get; set; }
		public string PackageContMtrlCode { get; set; }

	}
}
