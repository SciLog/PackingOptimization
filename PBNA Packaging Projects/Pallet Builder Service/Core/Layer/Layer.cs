using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Layer
	{
		public Package Package { get; set; }

		public string PickMethodCode { get; set; }
		public int PickSequence { get; set; } 

		public int LayerNumber { get; set; } 

		public string Zone { get; set; }

		public int Cell { get; set; }
	}
}
