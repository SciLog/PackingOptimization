using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderLineRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_ORDERLINES_FOR_ORDER_SQL = @"
	 		SELECT
				pl.plto_id AS OrderId,   
				pl.plto_type_cde IdTypeCode, 
				pl.plto_lne_seq AS OrderLineId,
				pl.plto_lne_qty AS CustomerOrderableUnits,
				pl.inven_id AS SkuInventoryId,
				pl.plto_lne_actvy_cde AS LineActivityCode,
				pl.inven_type_cde AS SkuInventoryTypeCode,
				pl.plto_lne_create_dte AS LineCreateDate,
				DECODE(plto_direct_load_flg,'Y','TRUE','N','FALSE') AS IsDirectLoad,
				pl.plto_updt_cnt AS UpdateCount,
				pl.delete_flg,
				DECODE(plto_lne_crtcl_flg,'Y','TRUE','N','FALSE') AS IsCritical,
--				p.pkg_id AS SkuPackageId,
				ii.pkg_id AS SkuPackageId,
				pl.pal_type_cde AS SkuPalletTypeCode,
				pl.pal_case_qty AS PalletCaseQuantity,
				ii.nme AS SkuName
--				s.sply_type_cde AS SkuSupplyTypeCode,
--				NVL(prd.rfrg_reqd_flg, 'N') AS SkuIsChilled,
--				NVL(prd.prod_mix_cde,'000') AS SkuProductMixCode 
			FROM
				PLTO_LINE_PLACEHOLDER,
				inven_item ii
--				pkg p,
--				sply s,
--				prod prd
			WHERE
				pl.inven_id = ii.inven_id(+) AND
--				pl.inven_id = prd.inven_id(+) AND
--				ii.pkg_id = p.pkg_id(+) AND
				pl.delete_flg = 'N' AND
				pl.plto_id = :OrderId -- AND
--				s.inven_id(+) = ii.inven_id";
		
		/*
			<isNotEqual property="statusCode" compareValue="007">
				plto_lne pl,
			</isNotEqual>
			<isEqual property = "statusCode" compareValue="007">
				plto_prfl_lne pl,
			</isEqual>
		*/

		#endregion


		#region -- Public Methods --

		public List<OrderLine> GetOrderLines(string orderId, PalletOrderStatus palletOrderStatus)
		{
			string sql = GET_ORDERLINES_FOR_ORDER_SQL.Replace(
				"PLTO_LINE_PLACEHOLDER",
				palletOrderStatus.Code == PalletOrderStatus.SET_FOR_MANUAL_RELEASE.Code ? "plto_prfl_lne pl" : "plto_lne pl");


			return Select<OrderLine>
			(
				sql,
				new
				{
					OrderId = orderId,
				}
			);
		}

		#endregion

	}
}
