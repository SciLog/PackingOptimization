using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class LocationRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_LOCATION_INFO_SQL = @"
			SELECT
				ls.loc_id AS LocationId,
				ls.loc_full_shpcont_max_pct / 100.0 AS FullPalletMaxPct,
				ls.loc_full_shpcont_var_max_pct / 100.0 AS MaximumOverflow,
				ls.loc_shpcont_ustbl_pkg_max_qty AS MaximumUnstableQuantity,
				ls.loc_shpcont_ustbl_pkg_max_pct / 100.0 AS MaximumUnstablePct,
				NVL(ls.loc_shpcont_hgt_unit_qty, 5.25) AS DefaultPalletHeight,				
				NVL(ls.loc_shpcont_wdth_unit_qty, 40) AS DefaultPalletWidth,		
				NVL(ls.loc_shpcont_lngth_unit_qty, 48) AS DefaultPalletLength,	
				DECODE(loc_adv_pal_bld_flg,'Y','TRUE','N','FALSE') AS ApbFlag,
				automtn_min_layr_qty AS MinimumLayerAtmn,
				veh_bay_min_ful_pct AS DBayTruckMinimumFillPct,
				veh_bay_tall_btl_hgt_qty AS TallBottleHeight,
				NVL(veh_bay_high_sku_min_layr_num, 2) AS AbmMinimumLayerHighSku,
				NVL(veh_bay_low_sku_min_layr_num, 1.5) AS AbmMinimumLayerLowSku,
				DECODE(lop.ucv_coolr_ind,'Y','TRUE','N','FALSE') AS HasUcv,
				lop.topnsku_ful_pct AS TopNSkuPercent,
				lop.ord_bsd_ful_pct AS OrderBasePercent,
				lop.ord_cnt_pct AS OrderCountWeightage,
				lop.sku_cnt_pct AS SkuCountWeightage,
				lop.item_wght_pct AS TotalItemWeightWeightage,
				NVL(dpt.default_pallet_type,'002') AS DefaultNisPalletType,
				DECODE(lop.sap_ord_flg,'Y','TRUE','N','FALSE') AS SapOrderFlag,
				DECODE(lop.MERCH_PAL_ENBL_FLG,'Y','TRUE','N','FALSE') AS MerchandisePalletEnableFlag,
				DECODE(lop.trnsprt_seq_by_rte_enbl_flg,'Y','TRUE','N','FALSE') AS TransportSeparateByRouteEnableFlag,
				lop.trnsprt_seq_by_rte_sort_cde AS TransportSequenceByRouteSortOrder,
				DECODE(lop.rte_based_dely_enbl,'Y','TRUE','N','FALSE') AS RouteBasedDeliveryEnableFlag
			FROM
				loc_shpcont ls,
				loc_ofi_prfl lop,
				mv_default_pallet_type dpt
			WHERE 
				ls.loc_id = :locationId
				AND ls.loc_id = lop.loc_id
				AND ls.shpcont_type_cde = '001'
				AND ls.delete_flg = 'N'
				AND lop.delete_flg = 'N'
				AND :buildDate BETWEEN ls.eff_beg_dtm and ls.eff_end_dtm
				AND dpt.whse_code (+) = TO_CHAR(ls.loc_id)";

		#endregion


		#region -- Public methods --

		public Location GetLocationInfo(int locationId, DateTime date)
		{
			return Select<Location>
			(
				GET_LOCATION_INFO_SQL,
				new
				{
					LocationId = locationId,
					BuildDate = date,
				}
			).FirstOrDefault();
		}

		#endregion

	}
}
