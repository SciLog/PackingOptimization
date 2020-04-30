using System.Collections.Generic;


namespace ScientificLogistics.PalletBuilder.Core
{
	public class RuleRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_RULES_FOR_LOCATION_SQL = @"
            SELECT
                ac.loc_id AS LocationId,
                ac.plto_dely_mthd_cde AS DeliveryMethodCode,
                ac.plto_type_cde AS OrderType,
                r.om_bus_rul_id AS Code,
                -- r.om_bus_rul_set_id,
                -- r.om_bus_rul_nme,
                CASE
                    WHEN ex.bus_rul_actv_flg IS NOT NULL THEN ex.bus_rul_actv_flg
                    ELSE ac.bus_rul_actv_flg
                END AS ActiveFlag
            FROM
                om_bus_rul_ctrl ac LEFT OUTER JOIN om_ord_stat ex ON
                    ac.loc_id = ex.loc_id
                    AND ac.plto_type_cde = ex.plto_type_cde
                    AND ac.plto_dely_mthd_cde = ex.plto_dely_mthd_cde
                    AND ac.om_bus_rul_ctrl_id = ex.om_bus_rul_ctrl_id
                    AND ac.om_bus_rul_ctrl_id_lvl_cde = ex.om_bus_rul_ctrl_id_lvl_cde
                    AND ex.om_ord_stat_cde = :orderStatus
                    AND ex.om_ord_stat_type_cde = :orderType
                    AND ex.delete_flg = 'N'
                    AND trunc(sysdate) between ex.eff_beg_dtm and ex.eff_end_dtm,
                om_bus_rul_set rs,
                om_bus_rul r
            WHERE
                ac.loc_id = :locationId
                    AND ac.om_bus_rul_ctrl_id_lvl_cde = '001'
                    AND ac.om_bus_rul_ctrl_id = r.om_bus_rul_id
                    AND rs.om_bus_rul_set_id = r.om_bus_rul_set_id
                    AND ac.delete_flg = 'N'
                    AND r.delete_flg = 'N'
                    AND rs.delete_flg = 'N'
                    AND trunc(sysdate) between ac.eff_beg_dtm and ac.eff_end_dtm
                    AND trunc(sysdate) between r.eff_beg_dte and r.eff_end_dte
                    AND trunc(sysdate) between rs.eff_beg_dte and rs.eff_end_dte";

		#endregion


		#region -- Public Methods --

		public List<Rule> GetRules(int locationId, string palletOrderStatusCode)
		{
			PalletOrderStatus palletOrderStatus = PalletOrderStatus.Find(palletOrderStatusCode);

			return GetRules(
				locationId,
				palletOrderStatus?.Code ?? "",
				palletOrderStatus != null ? "OMS" : "INT");
		}


		public List<Rule> GetRules(int locationId)
		{
			return GetRules(locationId, PalletOrderStatus.New.Code);
		}

		#endregion

		#region -- Private Methods --

		private List<Rule> GetRules(int locationId, string orderStatus, string orderType)
		{
			return Select<Rule>
			(
				GET_RULES_FOR_LOCATION_SQL,
				new
				{
					LocationId = locationId,
					OrderStatus = orderStatus,
					OrderType = orderType
				}
			);
		}

		#endregion
	}
}
