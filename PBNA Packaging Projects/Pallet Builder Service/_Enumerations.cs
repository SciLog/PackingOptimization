using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public enum PalletType
	{
		Unknown = 0,
		SingleSkuFullPallet = 1,
		SinglePackageFullPallet = 2,
		MultiPackageFullPallet = 3,
		MultiPackageNonFullPallet = 4
	}
}
