using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class ItemRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_DEFAULT_ITEM_CONFIG_SQL = @"
			SELECT
			  t.pkg_wdth_qty AS Width,
			  t.pkg_hgt_qty AS Height,
			  t.pkg_lngth_qty AS Length,
			  t.pkg_wgt_qty AS Weight,
			  -- t.pkg_dim_uofm_cde,
			  t.cust_ordbl_un_percs_qty AS CostumerOrderDblUintsPerCase,
			  t.pkg_type_id AS PackageTypeId,
			  t.pkg_prity_cde AS PackagePriority,
			  t.pkg_cpl_qty AS PackageCasesPerLayer,
			  t.pkg_lpp_qty AS PackageLayersPerPallet,
			  t.pkg_cpp_qty AS PackageFullPalletQuantity
			FROM oms_o.pkg_dflt_config t
			WHERE 
			 t.delete_flg = 'N'";

		#endregion


		#region -- Public Methods --

		public Item GetDefaultConfig(int inventoryId)
		{
			Item item = Select<Item>(GET_DEFAULT_ITEM_CONFIG_SQL)
				.FirstOrDefault();

			if (item != null) { item.InventoryId = inventoryId; }

			return item;
		}

		#endregion
	}
}
