using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderLineSlottingComparator : IComparer<KeyValuePair<OrderLine, Slotting>> {

		public int Compare(KeyValuePair<OrderLine, Slotting> entry1, KeyValuePair<OrderLine, Slotting> entry2)
		{
			return entry1.Value.PickSequence.CompareTo(entry2.Value.PickSequence);
		}

	}
}
