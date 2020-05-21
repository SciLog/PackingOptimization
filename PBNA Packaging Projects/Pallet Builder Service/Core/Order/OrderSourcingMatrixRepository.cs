using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderSourcingMatrixRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_ORDER_SOURCING_MATRIX_SQL = @"
			SELECT  
				ord_srcg_mtrx_id AS Id,
				o.loc_id AS SellingLocationId,
				caldr_day_in_wk_num AS DayOfWeek,
				plto_dely_mthd_cde AS DeliveryMethodCode,
				o.pick_mthd_cde AS PickMethodCode,
				loc_bld_id AS BuildLocationId,
				loc_actvy_cde AS LocationActivityCode,
				loc_nxt_id AS NextLocationId
			FROM
				ord_srcg_mtrx o, 
				loc_pick_cpblty l
			WHERE
				:buildDate between o.eff_beg_dtm and o.eff_end_dtm AND
				:locationId = o.loc_id AND
				:deliveryMethodCode = o.plto_dely_mthd_cde AND 
				TO_CHAR(:buildDate,'D') = o.CALDR_DAY_IN_WK_NUM and
				o.delete_flg = 'N' and 
				o.loc_bld_id = l.loc_id  and 
				o.pick_mthd_cde = l.pick_mthd_cde  and
				:buildDate between l.eff_beg_dtm and l.eff_end_dtm and
				l.delete_flg = 'N' ";

		#endregion


		#region -- Public methods --

		public List<OrderSourcingMatrix> GetOrderSourcingInfo(int locationId, string deliveryMethodCode, DateTime buildDate)
		{
			return Select<OrderSourcingMatrix>
			(
				GET_ORDER_SOURCING_MATRIX_SQL,
				new
				{
					LocationId = locationId,
					BuildDate = buildDate,
					DeliveryMethodCode = deliveryMethodCode
				}
			);
		}

		#endregion

	}
}
