using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Layer
	{
		List<PalletItem> _palletItemList = new List<PalletItem>();

		// ---

		public Layer(String pickMethodCode, String buildMethod, Package package)
		{
			PickMethodCode = pickMethodCode;
			Zone = buildMethod;
			Package = package;
		}

		// ---

		public Package Package { get; set; }

		public string PickMethodCode { get; set; }
		public int? PickSequence { get; set; } 

		public int LayerNumber { get; set; } 

		public string Zone { get; set; }

		public int? Cell { get; set; }

		// ---

		public void AddPalletItem(PalletItem palletItem)
		{
			PickSequence = palletItem?.Slotting.PickSequence;
			Cell = palletItem?.Slotting?.Cell;

			_palletItemList.Add(palletItem);
		}

	}
}
