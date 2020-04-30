using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class SlottingRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_SLOTTING_FOR_LOCATION_SQL = @"
			SELECT
				ws.loc_id AS BuildLocationId,
				:buildDate AS BuildDate,
				ws.inven_id AS InventoryId,
--				item.pkg_id AS PackageId,
				ws.whse_pick_area_cde AS PickAreaCode,
				ws.pick_mthd_cde AS PickMethodCode,
				ws.pick_dirctn_cde AS PickDirectionCode,
				ws.whse_pick_zone_cde AS PickZoneCode,
				ws.whse_rack_lvl_num AS ""Level"",
				ws.whse_aisle_num AS Aisle,
				ws.whse_aisle_side_cde AS SideCode,
				ws.whse_cell_num AS Cell,
				ws.whse_bay_num AS Bay,
				ws.whse_slot_num AS Slot,
				ws.inven_item_pick_seq AS PickSequence,
				ws.eff_beg_dtm AS EffecitveDateFrom,
				ws.eff_end_dtm AS EffectiveDateTo
			FROM
				whse_slot ws
--				,inven_item item
			WHERE
				 ws.loc_id = :locationId
				 and :buildDate between ws.eff_beg_dtm and ws.eff_end_dtm
				 and ws.delete_flg = 'N'
--				  and item.inven_id = ws.inven_id
--				  and item.delete_flg = 'N'
			ORDER BY
				ws.whse_cell_num,
				ws.inven_item_pick_seq,
				ws.whse_slot_num";

		private const string GET_SLOTTING_FOR_LOCATION_SQL_2 = @"
			SELECT
				ws.loc_id AS BuildLocationId,
				:buildDate AS BuildDate,
				ws.inven_id AS InventoryId,
--				item.pkg_id AS PackageId,
				ws.whse_pick_area_cde AS PickAreaCode,
				ws.pick_mthd_cde AS PickMethodCode,
				ws.pick_dirctn_cde AS PickDirectionCode,
				ws.whse_pick_zone_cde AS PickZoneCode,
				ws.whse_rack_lvl_num AS ""Level"",
				ws.whse_aisle_num AS Aisle,
				ws.whse_aisle_side_cde AS SideCode,
				ws.whse_cell_num AS Cell,
				ws.whse_bay_num AS Bay,
				ws.whse_slot_num AS Slot,
				ws.inven_item_pick_seq AS PickSequence,
				ws.eff_beg_dtm AS EffecitveDateFrom,
				ws.eff_end_dtm AS EffectiveDateTo
			FROM
				whse_slot ws
--				,inven_item item
			WHERE
				 ws.loc_id = :locationId
				 and :buildDate between ws.eff_beg_dtm and ws.eff_end_dtm
				 and ws.delete_flg = 'N'
			 	 and ws.whse_pick_zone_cde = :buildMethodCode
			 	 and ws.pick_mthd_cde = :pickMethodCode
			     and ws.dely_mthd_cde IN :deliveryMethodCodes
--				  and item.inven_id = ws.inven_id
--				  and item.delete_flg = 'N'
			ORDER BY
				ws.whse_cell_num,
				ws.inven_item_pick_seq,
				ws.whse_slot_num";

		#endregion


		#region -- Public methods --

		public List<Slotting> GetSlottingsForLocation(int locationId, DateTime buildDate)
		{
			return Select<Slotting>
			(
				GET_SLOTTING_FOR_LOCATION_SQL,
				new
				{
					LocationId = locationId,
					BuildDate = buildDate,
				}
			);
		}

		public List<Slotting> GetSlottingsForLocation(List<BuildMethod> buildMethods, string deliveryMethodCode)
		{
			List<string> deliveryMethodCodes = new List<string>
			{
				SlottingDeliveryMethodCode._000,
				SlottingDeliveryMethodCode.Any,
				deliveryMethodCode
			};

			List<Slotting> slottings = new List<Slotting>();

			foreach(BuildMethod buildMethod in buildMethods)
			{
				slottings.AddRange
				(
					Select<Slotting>
					(
						GET_SLOTTING_FOR_LOCATION_SQL_2,
						new
						{
							buildMethod.LocationId,
							buildMethod.BuildDate,
							buildMethod.BuildMethodCode,
							buildMethod.PickMethodCode,
							DeliveryMethodCodes = deliveryMethodCodes
						}
					)			
				);
			}

			return slottings;
		}


		#endregion

	}
}
