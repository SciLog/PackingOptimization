using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class ItemRepository : RepositoryBase
	{
		#region -- Constants --

		private const string GET_ITEMS_FOR_LOCATION_SQL = @"
		select
			  loc_items.loc_id AS LocationId
			  , loc_items.inven_id AS InventoryId
			  , loc_items.inven_type_cde AS InventoryTypeCode
			  , loc_items.pkg_id AS PackageId
              , loc_items.cust_ordbl_units_per_case AS CustomerOrderableUintsPerCase
              , loc_items.pallet_type AS PalletTypeCode
              , loc_items.dflt_pallet_type_ind AS IsDefaultPallet
              , loc_items.starter_pallet_ind AS IsStarterPallet
			  , pkg_prity_cde AS PackagePriority
			  , decode(loc_items.pkg_id, 1098, 'True', 1333, 'False', 'False') as PackageIsUnstableInBay
			  , loc_items.full_pal_cpp AS FpFullPalletQuantity
			  , loc_items.full_pal_cpl AS FpCasesPerLayer
			  , loc_items.full_pal_lpp AS FpLayersPerPallet
			  , mixed_pal_cpl AS PackageCasesPerLayer
			  , mixed_pal_lpp AS PackageLayersPerPallet
			  , mixed_pal_cpp AS PackageFullPalletQuantity
			  , mixed_pal_lpp_dbay as PackageDbayLayersPerPallet
			  , 0 as pkg_type_id
              , height_qty AS Height
              , width_qty AS Width
              , length_qty AS Length
			  , weight AS Weight
			  , 0  AS StackFactor
			  , 0  AS PackageIdOverride
			  , grp_id AS InventoryGroupId
			  , brand_layer_seq AS BrandLayerSequence
			  , loc_items.pallet_item_no AS PalletItemNumber
--			  , pkg.cont_cde AS PackageContCode
--			  , pkg.pkg_cont_mtrl_cde AS PackagePackageContMtrlCode
--			  , brnd.ctgy_type_cde AS CategoryTypeCode
			  , loc_items.brnd_id AS BrandId
		from
			mv_ofi_loc_inven loc_items
--			, pkg
--			, brnd
		where 
			loc_items.loc_id = :locationId
--            and pkg.pkg_id(+) = loc_items.pkg_id
--		      and brnd.brnd_id(+) = loc_items.brnd_id
";

      //      <isNotNull prepend = "AND" property="getOnlyDfltPalletType">
      //      	dflt_pallet_type_ind = 1
      //      </isNotNull>
      //      <isNotNull prepend = "AND" property="invenId">
      //         inven_id = #invenId#
      //      </isNotNull>
      //      <isNotNull prepend = "AND" property="palTypeCde">
      //         pallet_type = #palTypeCde#
      //      </isNotNull>

		private const string GET_DEFAULT_ITEM_CONFIG_SQL = @"
			SELECT
			  t.pkg_wdth_qty AS Width,
			  t.pkg_hgt_qty AS Height,
			  t.pkg_lngth_qty AS Length,
			  t.pkg_wgt_qty AS Weight,
			  -- t.pkg_dim_uofm_cde,
			  t.cust_ordbl_un_percs_qty AS CustomerOrderableUintsPerCase,
			  t.pkg_type_id AS PackageTypeId,
			  t.pkg_prity_cde AS PackagePriority,
			  t.pkg_cpl_qty AS PackageCasesPerLayer,
			  t.pkg_lpp_qty AS PackageLayersPerPallet,
			  t.pkg_cpp_qty AS PackageFullPalletQuantity
			FROM 
              oms_o.pkg_dflt_config t
			WHERE 
              t.delete_flg = 'N'";

		#endregion


		#region -- Public Methods --

		public List<Item> GetItemsForLocation(int locationId)
		{
			// ***** inventoryId ignored in Java code. *****

			return Select<Item>
			(
				GET_ITEMS_FOR_LOCATION_SQL,
				new
				{
					LocationId = locationId
				}
			);
		}

		public Item GetDefaultConfig(int inventoryId)
		{
			// ***** inventoryId ignored in Java code. *****

			Item item = Select<Item>(GET_DEFAULT_ITEM_CONFIG_SQL)
				.FirstOrDefault();

			if (item != null) { item.InventoryId = inventoryId; }

			return item;
		}

		#endregion
	}
}
