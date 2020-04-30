using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Unstackable
	{
		public int TopPackageId { get; set; }
		public int BottomPackageId { get; set; }
		public int PackageIdToRemove { get; set; }
	}
}
