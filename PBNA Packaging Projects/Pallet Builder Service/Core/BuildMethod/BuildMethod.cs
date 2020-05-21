using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class BuildMethod
	{
		#region -- Public Properties --

		public int LocationId { get; set; }

		public DateTime BuildDate { get; set; }

		public string PickMethodCode { get; set; }
		public string BuildMethodCode { get; set; }

		public int MinimumNumberOfLayers { get; set; }
		public double MaximumPalletHeight { get; set; }

		#endregion
	}
}
