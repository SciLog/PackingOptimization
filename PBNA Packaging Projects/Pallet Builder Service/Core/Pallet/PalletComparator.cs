using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class PalletComparator : IComparer<Pallet>
	{

		public int Compare(Pallet pallet1, Pallet pallet2)
		{
			return pallet1.GetPercentageFull().CompareTo(pallet2.GetPercentageFull());
		}

	}
}
