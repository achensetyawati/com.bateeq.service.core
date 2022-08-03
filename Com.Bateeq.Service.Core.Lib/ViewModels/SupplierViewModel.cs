using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class SupplierViewModel : BasicViewModelOld
    {
        public string code { get; set; }

        public string name { get; set; }

        public string address { get; set; }

        public string contact { get; set; }

        public string PIC { get; set; }

        /* Bool */
        public dynamic import { get; set; }

        public string NPWP { get; set; }

        public string serialNumber { get; set; }
    }
}
