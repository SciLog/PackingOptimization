using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class UnstackableRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_UNSTACKABLES_SQL = @"
			SELECT
				top_pkg_id AS TopPackageId
				, btm_pkg_id AS BottomPackageId
				, pkg_remv_num AS PackageIdToRemove
			FROM
				pkg_unstkbl
			WHERE
				loc_id = :locationId
				AND delete_flg = 'N'";

		#endregion


		#region -- Public Methods --

		public List<Unstackable> GetForLocation(int locationId)
		{
			return Select<Unstackable>
			(
				GET_UNSTACKABLES_SQL,
				new
				{
					LocationId = locationId
				}
			);
		}

		#endregion
	}
}
