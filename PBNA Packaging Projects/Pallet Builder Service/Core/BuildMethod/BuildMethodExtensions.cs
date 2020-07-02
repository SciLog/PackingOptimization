using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class BuildMethodExtensions
	{
		public static int GetBuildLocationIdForPickMethod(this List<BuildMethod> buildMethods, string pickMethodCode)
		{
			return buildMethods
				?.FirstOrDefault(bm => bm.PickMethodCode == pickMethodCode)
				?.LocationId ?? 0;
		}
	}
}
