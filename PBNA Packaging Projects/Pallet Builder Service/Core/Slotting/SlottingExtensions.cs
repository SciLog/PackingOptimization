using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class SlottingExtensions
	{
		public static Slotting FindFirst(this List<Slotting> slottings, int inventoryId) =>
			slottings.FirstOrDefault(s => s.InventoryId == inventoryId);

		public static Slotting FindFirst(this List<Slotting> slottings, int inventoryId, int cell) =>
			slottings.FirstOrDefault(s =>
				s.InventoryId == inventoryId &&
				s.Cell == cell);

		// ---

		public static List<Slotting> FindAll(this List<Slotting> slottings, int inventoryId) =>
			slottings.Where(s => s.InventoryId == inventoryId).ToList();

		public static List<Slotting> FindAll(this List<Slotting> slottings, string pickMethodCode) =>
			slottings.Where(s => s.PickMethodCode == pickMethodCode).ToList();

		// ---

		public static bool Contains(this List<Slotting> slottings, int inventoryId) =>
			slottings.Any(s => s.InventoryId == inventoryId);

		// ---

		public static void removeSlottingForDefaultItems( this List<Slotting> slottings, DefaultNotification defaultNotification)
		{
			foreach(Item item in defaultNotification?.DefaultedItems?.OfType<Item>() ?? Enumerable.Empty<Item>())
			{
				slottings.RemoveNonEaches(item.InventoryId);
			}
		}

		public static bool RemoveNonEaches(this List<Slotting> slottings, int inventoryId) =>
			slottings.RemoveAll(s =>
					s.InventoryId == inventoryId &&
					s.PickMethodCode != PickMethodCode.Eaches &&
					s.PickMethodCode != PickMethodCode.Chilled) > 0;

		public static void RemoveAutomationSlottingForItems(this List<Slotting> slottings, List<OrderLine> orderLines)
		{
			foreach (OrderLine orderLine in orderLines)
			{
				slottings.RemoveNonEaches(orderLine.Sku.InventoryId);
			}
		}

	}
}
