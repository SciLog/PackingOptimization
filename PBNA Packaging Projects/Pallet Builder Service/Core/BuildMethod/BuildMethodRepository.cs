using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class BuildMethodRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_BUILD_METHOD_FOR_LOCATION_SQL = @"
			SELECT :locationId AS LocationId, 
				   :buildDate AS BuildDate, 
				   :pickMethodCode AS PickMethodCode, 
				   shpcont_bldmthd_cde AS BuildMethodCode, 
				   shpcont_bldmthd_layr_min_qty AS MinimumNumberOfLayers, 
				   shpcont_bldmthd_max_hgt_in_qty AS MaximumPalletHeight
			FROM
				(SELECT 
		    		lpbc.loc_id, 
		    		lpbc.shpcont_bldmthd_cde, 
		    		nvl(lsbc.SHPCONT_BLDMTHD_LAYR_MIN_QTY, 0) as SHPCONT_BLDMTHD_LAYR_MIN_QTY, 
		    		nvl(lsbc.SHPCONT_BLDMTHD_MAX_HGT_IN_QTY, 0) as shpcont_bldmthd_max_hgt_in_qty
				FROM 
					loc_pick_bld_cpblty lpbc,
					loc_shpcont_bld_cpblty lsbc
				WHERE
					lpbc.loc_id = :locationId
					and :buildDate between lpbc.eff_beg_dtm and lpbc.eff_end_dtm
					and lpbc.pick_mthd_cde = :pickMethodCode
					and lpbc.delete_flg= 'N'
					and lsbc.delete_flg = 'N'
					and lsbc.loc_id(+) = lpbc.loc_id
					and lsbc.shpcont_bldmthd_cde(+) = lpbc.shpcont_bldmthd_cde 
					and :buildDate between lsbc.eff_beg_dtm(+) and lsbc.eff_end_dtm(+) 
				order by
					lpbc.loc_pick_bld_prity_seq) a
			WHERE 
				rownum = 1";

		#endregion


		#region -- Public methods --

		public List<BuildMethod> GetBuildMethodsForLocation(DateTime buildDate, List<OrderSourcingMatrix> orderSourcingMatrices)
		{
			List<BuildMethod> buildMethods = new List<BuildMethod>();

			foreach(OrderSourcingMatrix orderSourcingMatrix in orderSourcingMatrices)
			{
				buildMethods.AddRange
				(
					Select<BuildMethod>
					(
						GET_BUILD_METHOD_FOR_LOCATION_SQL,
						new
						{
							BuildDate = buildDate,
							LocationId = orderSourcingMatrix.BuildLocationId,
							orderSourcingMatrix.PickMethodCode						// Name is inferred
						}
					)
				);
			}

			return buildMethods;
		}

		#endregion
	}
}
