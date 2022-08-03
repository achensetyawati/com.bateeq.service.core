using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class CurrencyViewModel : BasicViewModel
    {

        public string Code { get; set; }

        public string Symbol { get; set; }

        /* Double */
        public dynamic Rate { get; set; }

        public string Description { get; set; }
    }
}
