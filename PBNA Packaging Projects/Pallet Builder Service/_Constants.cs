using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public static class Constants
	{
		public static string RULE_ID_INDUCT_CHECK_DELVY_DTE = "2"; // Check delivery date
		public static string RULE_ID_AUTOMATION = "44";
	}

	public static class PickMethodCode
	{
		public const string FullPallet = "001";
		public const string FullLayer = "002";
		public const string MixedLayer = "003";
		public const string Eaches = "004";
		public const string Chilled = "005";
	}

	public static class BuildMethodCode
	{
		public const string HSLP = "001";
		public const string MXLP = "002";
		public const string CLAW = "003";
		public const string MANL = "004";
		public const string FULL = "005";
		public const string STRT = "006";
		public const string CHLP = "007";
	}

	public static class InventoryTypeCode
	{
		public const string Produce = "PRD";
		public const string Supply = "SUP";
	}

	public static class PalletOrderTypeCode
	{
		public const string Customer = "001";
		public const string Load = "002";
		public const string Replinish = "003";
		public const string BOL = "004";
		public const string PSFP = "005";
		public const string PBKP = "006";
		public const string HBOL = "007";
	}


	public static class DeliveryMethodCode
	{
		public const string Bay = "001";	
		public const string Bulk = "003";
		public const string BOL = "007";

	}

	public static class SlottingDeliveryMethodCode
	{
		public const string Any = "ANY";
		public const string _000 = "000";
	}
}
