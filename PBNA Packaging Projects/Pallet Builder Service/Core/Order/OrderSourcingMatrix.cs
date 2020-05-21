using System;
using System.Collections.Generic;
using System.Text;

namespace ScientificLogistics.PalletBuilder.Core
{
	public class OrderSourcingMatrix
	{
		public int Id { get; set; }

		public int DayOfWeek { get; set; }

		public int SellingLocationId { get; set; }
		public int DeliveryLocationId { get; set; }
		public int BuildLocationId { get; set; }
		public int NextLocationId { get; set; }

		public string DeliveryMethodCode { get; set; }
		public string PickMethodCode { get; set; }
		public string LocationActivityCode { get; set; }
	}
}
