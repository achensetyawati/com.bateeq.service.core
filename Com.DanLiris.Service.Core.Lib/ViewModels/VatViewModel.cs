using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
	public class VatViewModel : BasicViewModel
	{
		public string Name { get; set; }

		public double? Rate { get; set; }

		public string Description { get; set; }

		public DateTimeOffset? Date { get; set; }
	}
}
