using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public class PalletOrderStatus
	{
		private static Dictionary<string, PalletOrderStatus> _statusList = new Dictionary<string, PalletOrderStatus>
		{
			{"001", New},
			{"002", RESCHEDULED},
			{"003", CANCELED},
			{"004", ERROR},
			{"005", HOLD},
			{"006", WMS_NOTIFIED_CANCEL},
			{"007", SET_FOR_MANUAL_RELEASE},
			{"008", RELEASED},
			{"009", WAITING},
			{"010", REPROCESSING_DONE},
			{"011", MANUAL_RELEASE_FULFILLED},
			{"012", WAITING_FOR_ACCEPT_REJECT},
			{"013", RELEASED_PENDING_TRNSPRT},
			{"014", PRE_RELEASED_PENDING_TRNSPRT},
			{"015", PARTIAL_WORKUNIT_RELEASED},
			{"016", SET_AUTO_PRE_RELEASE},
			{"017", AUTO_PRE_RELEASE_PROCESSING_DONE},
			{"018", AUTO_PRE_RELEASED_PEND_TRNPT},
			{"019", AUTO_PRE_RELEASED}
		};

		private PalletOrderStatus(string code, string description)
		{
			Code = code;
			Description = description;
		}

		public string Code { get; private set; }

		public string Description { get; private set; }

		public static PalletOrderStatus Find(string code)
		{
			return _statusList.TryGetValue(code, out PalletOrderStatus palletOrderStatus) ? palletOrderStatus : null;
		}

		public static PalletOrderStatus New { get; } = new PalletOrderStatus("001", "New Order");
		public static PalletOrderStatus RESCHEDULED { get; } = new PalletOrderStatus("002", "Rescheduled Order");
		public static PalletOrderStatus CANCELED { get; } = new PalletOrderStatus("003", "Canceled Order");
		public static PalletOrderStatus ERROR { get; } = new PalletOrderStatus("004", "Errored Order");
		public static PalletOrderStatus HOLD { get; } = new PalletOrderStatus("005", "Held Order");
		public static PalletOrderStatus WMS_NOTIFIED_CANCEL { get; } = new PalletOrderStatus("006", "WMS notified of Cancel");
		public static PalletOrderStatus SET_FOR_MANUAL_RELEASE { get; } = new PalletOrderStatus("007", "Set for Manual release");
		public static PalletOrderStatus RELEASED { get; } = new PalletOrderStatus("008", "Released Order");
		public static PalletOrderStatus WAITING { get; } = new PalletOrderStatus("009", "Waiting for routing");
		public static PalletOrderStatus REPROCESSING_DONE { get; } = new PalletOrderStatus("010", "Recontainerization complete");
		public static PalletOrderStatus MANUAL_RELEASE_FULFILLED { get; } = new PalletOrderStatus("011", "Manual Release fulfilled");
		public static PalletOrderStatus WAITING_FOR_ACCEPT_REJECT { get; } = new PalletOrderStatus("012", "Waiting for Accept Reject");
		public static PalletOrderStatus RELEASED_PENDING_TRNSPRT { get; } = new PalletOrderStatus("013", "Released pending transport");
		public static PalletOrderStatus PRE_RELEASED_PENDING_TRNSPRT { get; } = new PalletOrderStatus("014", "Prereleased pending transport");
		public static PalletOrderStatus PARTIAL_WORKUNIT_RELEASED { get; } = new PalletOrderStatus("015", "Partial Work Unit Released");
		public static PalletOrderStatus SET_AUTO_PRE_RELEASE { get; } = new PalletOrderStatus("016", "Set for Auto Pre-release");
		public static PalletOrderStatus AUTO_PRE_RELEASE_PROCESSING_DONE { get; } = new PalletOrderStatus("017", "Auto Pre-release processing done");
		public static PalletOrderStatus AUTO_PRE_RELEASED_PEND_TRNPT { get; } = new PalletOrderStatus("018", "Auto Pre-released pending transport");
		public static PalletOrderStatus AUTO_PRE_RELEASED { get; } = new PalletOrderStatus("019", "Auto Pre-released");
	}

}
