using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public static class Constants
	{
		public static string INDUCT_CHECK_DELVY_DTE = "2"; // Check delivery date
		public static string AUTOMATION = "44";
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

	public static class PalletTypeCode
	{
		// Half Pallet Types
		public const string StandardDispatch = "003";
		public const string HalfDispatch = "001";
		public const string HalfNIS = "056";
		
		// Full Pallet Types
		public const string ChilledNIS = "058";
		public const string CO2NIS = "022";
		public const string StandardNIS = "002";
		public const string StandardNISCodeNull = null;
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

	public static class ItemTypeCode
	{
		public const string BIBProductMix = "002";
		public const string C02Supply = "001";
		public const string POPSupply = "013"; 
	}

	public static class OrderLineTypeCode
	{
		public const string Chilled = "C";
		public const string Ambient = "A";
		public const string Supply = "S";
		public const string CO2 = "C2";
		public const string BIB = "BIB";
		public const string POP = "POP";
		public const string All = "ALL";
	}

	public static class RuleTypeCode
	{
		public const string InductSetBuildDate							= "1";		// Set build date
		public const string InductCheckDeliveryDate						= "2";		// Check delivery date
		public const string SplitGetPickMethods 						= "4";		// Get pick methods for order
		public const string InductSetCentralizedBulkDeliveryLocation 	= "5";		// Set centralized bulk delivery location
		public const string PreprocessCheckUpdateCutoff					= "6";		// Check cutoff for updated order
		public const string PreprocessChanges				  			= "7";		// Check cutoff for updated order
		public const string PrtzSetPriority								= "8";		// Set order priority	
		public const string PrtzSetPalletBuildTime						= "9";		// Set pallet complexity factor	
		public const string PrtzSetLatestBuildTime						= "10";		// Set latest build by time 	
		public const string PrtzAdjustAbcPriority						= "11";		// Adjust ABC loads priority
		public const string CheckHoldRelease							= "12";
		public const string PreprocessRejectEdit						= "13";		// Reject edit when order is locked
		public const string CheckReleaseToWMS				 			= "16";
		public const string BayDispatchConvert							= "17";		// Dispatch bay solution conversion for WMS
		public const string CheckReusePallets				 			= "18";	
		public const string LoadBalanceHSLP					 			= "19";
		public const string CheckDispatcherInitiatedRelease				= "21";
		public const string ComparePallets				 				= "23";	
		public const string BayDataCheck				 				= "24";
		public const string RejectReleasedABOL							= "25";  
		public const string CheckAbmAcceptReject				 		= "26";
		public const string PrtzAdjustPalletNumbers					 	= "27";
		public const string AbmFullPallets					 			= "28";		// Do full pallets during auto bay mapping
		public const string AbmPostProcess					 			= "29";		// Do post processing for auto bay mapping
		public const string LoadAccumulate				 				= "31";
		public const string SystemFlush									= "32";
		public const string HandleMerchandisePallet						= "33";
		public const string CheckReleaseToVoide				 			= "34";
		public const string NoUniquePalletTransportId					= "35";		// Do not assign unique pallet ids to transport
		public const string ProcessHalfPallet							= "36";		// Do process half pallets for customers that are eligible
		public const string ProcessHalfAndStandardPallet				= "37";		// Do process half and standard pallet solutions
		public const string AutoPreRelease								= "38";
		public const string DoAbmPackageSplitFix						= "39";
		public const string Chilled				        				= "40";
		public const string SprintBib				         			= "41";
		public const string AbmAdjust				         			= "42";
		public const string CO2Aggregation				     			= "43";
		public const string Automation				     				= "44";
		public const string DOC          								= "45";
		public const string CreateVirtualBay							= "48";
		public const string TopNSku					                    = "49";
		public const string PalletComplexity				            = "50";
		public const string GWMSLocation								= "52";
//		@Deprecated public const string INDUCT_FLIP		= "3"; // Flip delivery method
//		@Deprecated public const string BAY_MAP_FP_SKU 	= "14"; // Dbay load full pallet sku
//		@Deprecated public const string BAY_MAP_FB_SKU 	= "15"; // Dbay load full bay sku
//		@Deprecated public const string BAY_HANDLE_C02 	= "22";	
	}

	public static class PalletizationOptionCode
	{
		public const string LP_AND_HP 	= "001";
		public const string LP 			= "002";
		public const string HP 			= "003";
		public const string RuleBased	= "004";

	}

	public static class ChilledDefault
	{
		public const int CasesPerLayer = 10;
		public const int LayersPerPallet = 5;
		public const int CasesPerPallet = CasesPerLayer * LayersPerPallet;
	}


}
