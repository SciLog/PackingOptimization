using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class Slotting
	{
		public int BuildLocationId { get; set; }
		public DateTime BuildDate { get; set; }

		public int InventoryId { get; set; }
		public int PackageId { get; set; }

		public string PickAreaCode { get; set; }
		public string PickMethodCode { get; set; }
		public string PickDirectionCode { get; set; }
		public string PickZoneCode { get; set; }
		
		public int Level { get; set; }
		public int Aisle { get; set; }
		public string SideCode { get; set; }
		public int Cell { get; set; }
		public int Bay { get; set; }
		public int Slot { get; set; }
			
		public int PickSequence { get; set; }

		public DateTime EffecitveDateFrom { get; set; }
		public DateTime EffectiveDateTo { get; set; }

		public static Slotting GetDefaultForChilled(int buildLocationId, int inventoryId, Order order) =>
			new Slotting
			{
				BuildLocationId = buildLocationId,
				InventoryId = inventoryId,
				BuildDate = order.BuildDate,
				PickDirectionCode = "---",
				PickAreaCode = "006",
				PickMethodCode = PalletBuilder.PickMethodCode.Chilled,
				PickZoneCode = BuildMethodCode.CHLP,
				SideCode = "-",
				PickSequence = 2999
			};

		public static Slotting getDefaultForEaches(int eachesBuildLocationId, int inventoryId, Order order) =>
			new Slotting
			{
				BuildLocationId = eachesBuildLocationId,
				InventoryId = inventoryId,
				BuildDate = order.BuildDate,
				PickDirectionCode = "---",
				PickAreaCode = "006",
				PickMethodCode = PalletBuilder.PickMethodCode.Eaches,
				PickZoneCode = BuildMethodCode.MANL,
				SideCode = "-",
				PickSequence = 2999
			};
	}

}
