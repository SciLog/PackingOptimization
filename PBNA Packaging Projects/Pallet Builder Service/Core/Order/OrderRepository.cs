using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_ORDERS_FOR_LOCATION_SQL = @"
			SELECT
			   *  
			FROM
			(   SELECT 
					loc_id AS LocationId,
					CASE WHEN loc_dely_id IS NULL THEN loc_id ELSE loc_dely_id END DeliveryLocationId,
					loc_prmry_bld_id AS PrimaryBuildLocationId,
					plto_id AS OrderId,      
					plto_type_cde AS OrderTypeCode,          
					plto_type_cde AS IdTypeCode,
					load_id AS LoadId,
					o.cust_id AS CustomerId,
					plto_sched_dely_dte AS DeliveryDate,            
					o.prod_rte_id AS RouteId,
					dely_mthd_cde AS DeliveryMethodCode,
					orig_dely_mthd_cde AS OriginalDeliveryMethodCode,
					DECODE(is_flipped_dmc,'Y','TRUE','N','FALSE') AS IsFlippedDeliveryMethodCode,
					bol_ship_dte AS ShipDate,
					NULL AS BuildDate,
--					ord_stat_cde AS StatusCode,         
					SYSTIMESTAMP AT TIME ZONE 'US/Eastern' AS DbDate,
					o.row_last_updt_dtm AS UpdateDateTime,
					srce_row_last_updt_dtm AS SourceUpdateDatetime,
					plto_updt_cnt AS UpdateCount,
					TO_CHAR(O.ROW_LAST_UPDT_DTM,'YYYY-MM-DD-HH24:MI:SS:FF6') as UpdateDatetimeString,
--					(SELECT cust_opt_cde FROM CUST_OPT co 
--						WHERE co.cust_opt_cde = '022' AND o.cust_id = co.cust_id 
--						AND co.delete_flg = 'N') AS IsNight,
--					(SELECT cust_opt_cde FROM CUST_OPT co
--						WHERE co.cust_opt_cde = '032' AND o.cust_id = co.cust_id
--						AND co.delete_flg = 'N') AS CustomerIsStandardPalletOnly,    
--					(SELECT 'Y' from DUAL             
--								WHERE EXISTS              
--								(SELECT 1 FROM CUST_ASN_PRTCPNT cp 
--									WHERE 
--									cp.cust_id = o.cust_id 
--									AND  cp.delete_flg = 'N' 
--									AND  ((sysdate between CUST_ASN_BEG_DTE 
--											AND CUST_ASN_END_DTE) 
--											OR ( sysdate >= CUST_ASN_BEG_DTE 
--												AND CUST_ASN_END_DTE is null) ) 
--						)) CustomerIsAdvancedShippingNotice,
					CASE 
					WHEN dely_mthd_cde <> orig_dely_mthd_cde AND ord_stat_cde = 'RES' AND plto_type_cde = '001'
					THEN plto_type_cde || orig_dely_mthd_cde
					ELSE plto_type_cde || dely_mthd_cde
					END || CASE ord_stat_cde WHEN  '007' THEN '007'
					WHEN '016' THEN '016'
					ELSE '' END as type_dely_stat_combo,
--						a.veh_id AS veh_id,   
--						CASE WHEN o.ord_stat_cde = 'REJ' THEN 'N' ELSE a.abm_flag END abm_flag,
						job_id AS JobId,
						srce_srvr_nme AS SourceServerName,
                        DECODE(is_bld_id_ovrd,'Y','TRUE','N','FALSE') AS IsBuildIdOverriden,
						CASE 
						WHEN (SELECT dce.cust_id FROM dsptch_cust_excpt dce WHERE dce.cust_id = o.cust_id AND dce.delete_flg = 'N') IS NOT NULL THEN 'TRUE'
						else 'FALSE'
						END AS IsApbExempt,
						load_type_cde AS LoadTypeCode,
						DECODE(bab_ord_ind,'Y','TRUE','N','FALSE') AS IsBabOrderIndicator,
						DECODE(mv.MERCH_ACCT_FLG,'Y','TRUE','N','FALSE') AS CustomerIsMerchantAccount,
						c.DELY_PALZN_OPTN_CDE AS CustomerPalletizationOption
				FROM
					vw_plto o,
--					(SELECT 
--						dls.prod_rte_id, 
--						dls.pepsi_veh_id veh_id, 
--						vttv.auto_bay_map_enable_flg abm_flag 
--						FROM 
--							ace_abm_truck_v dls,
--							veh_tmplt_truck_v vttv 
--						WHERE 
--							dls.pepsi_veh_id = vttv.pepsi_veh_id
--							and vttv.LOC_ID=:locatinId ) a,
						mpm_cust_v mv,
            			ace_cust c	 
				WHERE
					LOCATION_ID_PLACEHOLDER                    
--					a.prod_rte_id (+) = o.plto_id and
                    LOAD_ID_PLACEHOLDER
					o.cust_id = mv.cust_id (+) and
					o.cust_id = c.cust_id (+)

				order by o.plto_sched_dely_dte, o.row_last_updt_dtm 
			)
--			where((ROWNUM < :maxOrders AND LoadId IS NULL) OR (LoadId IS NOT NULL))";

		private const string GET_PRIORITIZED_ORDERS_FOR_LOCATION_SQL = @"
		  SELECT
				CASE
				   WHEN o.plto_type_cde in ('004','003') THEN o.loc_prmry_bld_id
				   ELSE o.loc_prod_id
				END AS LocationId,
				loc_prmry_bld_id PrimaryBuildLocationId,
				loc_dely_id DeliveryLocationId,
				o.plto_id OrderId,            
				o.plto_type_cde OrderTypeCode,
				load_id LoadId,
				o.cust_id CustomerId,
				plto_dely_dte AS DeliveryDate,            
				prod_rte_id RouteId,
				plto_dely_mthd_cde as DeliveryMethodCode,
				orig_dely_mthd_cde OriginalDeliveryMethodCode,
--				ord_stat_cde StatusCode,    
				SYSTIMESTAMP AT TIME ZONE 'US/Eastern' AS DbDate,     
				o.row_last_updt_dtm AS UpdateDateTime,
				o.srce_row_last_updt_dtm SourceUpdateDatetime,  
				TO_CHAR(O.ROW_LAST_UPDT_DTM,'YYYY-MM-DD-HH24:MI:SS:FF6') as UpdateDatetimeString, 
				p.PLTO_PRITY_VAL Priority,
				p.PLTO_BLD_BY_DTM LatestBuildByTime,
				p.PLTO_BLD_START_DTE BuildDate,
				p.PLTO_STAT_CDE StatusCode,
				o.SRCE_SRVR_NME SourceServerName,
				p.PLTO_STAT_REAS_CDE ReasonCode,
				p.PLTO_STAT_REAS_TXT ReasonText,
				DECODE(plto_rlse_flg,'Y','TRUE','N','FALSE') AS IsReleased,
--			   (SELECT CUST_OPT_CDE FROM CUST_OPT CO 
--					WHERE CO.CUST_OPT_CDE = '022' AND o.CUST_ID = CO.CUST_ID 
--					AND CO.DELETE_FLG = 'N') AS CUST_NIGHT_FLG,
--			   (SELECT cust_opt_cde FROM CUST_OPT co
--					WHERE co.cust_opt_cde = '032' AND o.cust_id = co.cust_id
--					AND co.delete_flg = 'N') AS cust_full_pallet_flg,
				o.plto_type_cde || o.dely_mthd_cde as  TYPE_DELY_COMBO,
				p.plto_dely_dte,
				p.job_id JobId,
				CASE WHEN p.plto_stat_reas_cde = '0004' THEN 'Y' ELSE 'N' END AS is_flipped_dmc,
				CASE 
    				WHEN (SELECT dce.cust_id FROM dsptch_cust_excpt dce WHERE dce.cust_id = o.cust_id AND dce.delete_flg = 'N') IS NOT NULL THEN 'TRUE'
    				ELSE 'FALSE'
				END IsApbExempt,
				load_type_cde LoadTypeCode,
				DECODE(mv.MERCH_ACCT_FLG,'Y','TRUE','N','FALSE') AS CustomerIsMerchantAccount,
				c.DELY_PALZN_OPTN_CDE CustomerPalletizationOption
			FROM
				plto o,
				plto_prfl p,
				mpm_cust_v mv,
				ace_cust c
			WHERE
			   ( ( o.LOC_PROD_ID = :locationId ) 
				 OR 
			   ( o.plto_type_cde in ( '004','003') AND o.loc_prmry_bld_id = :locationId ) ) AND
				o.PLTO_ID = p.PLTO_ID AND
				o.PLTO_TYPE_CDE = p.PLTO_TYPE_CDE AND
				(o.PLTO_TYPE_CDE = '002' OR o.LOAD_ID is NULL) AND
				( p.PLTO_STAT_REAS_CDE is NULL OR
				  p.PLTO_STAT_REAS_CDE <> '0003' ) AND
				(( p.PLTO_STAT_CDE = '003' AND 
				  p.PLTO_RLSE_FLG = 'Y' ) OR
				  (p.PLTO_STAT_CDE in ('005','010','017'))
				  OR (p.PLTO_STAT_CDE in ( '007', '016') AND 
				  TRUNC(p.PLTO_BLD_START_DTE) <= TRUNC(SYSDATE))   
				   ) and
				o.cust_id = mv.cust_id (+) and
				o.cust_id = c.cust_id (+) and
                ROWNUM < 10
		   ORDER BY 
			   p.PLTO_PRITY_VAL,
			   p.PLTO_BLD_BY_DTM";

		#endregion


		#region -- Public Methods --

		public List<Order> GetOrdersForLocation(int locationId, string loadId = null, int maxOrders = 50)
		{
			string sql = GET_ORDERS_FOR_LOCATION_SQL.Replace(
				"LOCATION_ID_PLACEHOLDER",
				locationId <= 0 ? "" : $"o.loc_id = :locationId AND");

			sql = sql.Replace(
				"LOAD_ID_PLACEHOLDER",
				loadId is null ? "" : $"o.load_id = :loadId AND");

			List<Order> orders = Select<Order>
			(
				sql,
				new
				{
					LocationId = locationId,
					LoadId = loadId,
					MaxOrders = maxOrders
				}
			);

			
			// Load Order Lines

			foreach(Order order in orders)
			{
				order.OrderLines = new OrderLineRepository().GetOrderLines(order.OrderId, PalletOrderStatus.WAITING);
			}


			return orders;
		}

		public List<Order> GetPrioritizedWorkUnitsForLoc(int locationId)
		{
			List<Order> orders = Select<Order>
			(
				GET_PRIORITIZED_ORDERS_FOR_LOCATION_SQL,
				new
				{
					LocationId = locationId,
				}
			);

			
			// Load Order Lines

			foreach(Order order in orders)
			{
				order.OrderLines = new OrderLineRepository().GetOrderLines(order.OrderId, PalletOrderStatus.WAITING);
			}


			return orders;
		}

		#endregion
	}
}
