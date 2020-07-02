using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public static class SlottingExtensions
	{

		public static List<List<Slotting>> GetSlottingBySideList(this List<Slotting> slotting)
		{
			//this will return a list of list of slotting objects
			//first item in the list is the smallest side , next one bigger and so on
			Dictionary<string, List<Slotting>> slottingBySide = new Dictionary<string, List<Slotting>>();
			List<List<Slotting>> slottingLists = new List<List<Slotting>>();
			
			foreach (Slotting slot in slotting)
			{
				String side = slot.SideCode;
				List<Slotting> slottingList;

				if (slottingBySide.ContainsKey(side))
				{
					slottingList = slottingBySide[side];
				}
				else
				{
					slottingList = new List<Slotting>();
				}

				slottingList.Add(slot);
				slottingBySide[side] = slottingList;
			}

			//SortedDictionary<string, int> slottingBySideToSort = new SortedDictionary<string, int>();

			//foreach (KeyValuePair<string, List<Slotting>> entry in slottingBySide)
			//{
			//	slottingBySideToSort[entry.Key] = entry.Value.Count;
			//}

			//List<KeyValuePair<string, int>> sortedList = new List<KeyValuePair<string, int>>(slottingBySideToSort.entrySet());
			//Collections.sort(sortedList, new MapEntryComparatorObjInt());

			//for (Map.Entry<Object, Integer> entry : sortedList)
			//{
			//	slottingLists.add(slottingBySide.get(entry.getKey()));
			//}

			return slottingBySide
				.OrderBy(kvp => kvp.Value.Count)
				.Select(kvp => kvp.Value)
				.ToList();
		}


		// ---

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

		public static void RemoveSlottingForDefaultItems( this List<Slotting> slottings, DefaultNotification defaultNotification)
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

		// ---

		public static SortedDictionary<int, List<Slotting>> GroupByCell(this List<Slotting> slottings) =>
			new SortedDictionary<int, List<Slotting>>
			(
				slottings
					.GroupBy(s => s.Cell)
					.ToDictionary(g => g.Key, g => g.ToList())
			);

		public static Dictionary<int, List<Slotting>> GroupByCellExclusive(this List<Slotting> slottings) =>
			slottings
				.GroupBy(s => s.InventoryId)						// Group All Slots by Inventory Id
				.Where(g => g.GroupBy(s => s.Cell).Count() == 1)	// Determine if the Inventory Id is in more than 1 Cell
				.SelectMany(g => g)									// Select all Slotting with Inventory Id in only 1 cell
				.GroupBy(s => s.Cell)								// Group slottings only in 1 cell by 
				.ToDictionary(g => g.Key, g => g.ToList());


		//	List<Slotting> slottingList = new List<Slotting>();
		//	SortedDictionary<int, List<Slotting>> slottingByCell = new SortedDictionary<int, List<Slotting>>();

		//	foreach (Slotting slotting in slottings)
		//	{
		//		int cell = slotting.Cell;

		//		if (slottingByCell.ContainsKey(cell))
		//		{
		//			slottingByCell[cell].Add(slotting);
		//		}
		//		else
		//		{
		//			slottingByCell[cell] = new List<Slotting>
		//			{
		//				slotting
		//			};
		//		}
		//	}

		//	return slottingByCell;
		//}

	}
}
