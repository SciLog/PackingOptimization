using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class LayerExtensions
	{
		public static bool IsAutomated(this Layer layer)
		{
			return BuildMethod.IsAutoBuild(layer.Zone);
		}
	}
}
