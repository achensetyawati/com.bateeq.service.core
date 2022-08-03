using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class CategoryViewModel : BasicViewModelOld
    {
        public string code { get; set; }

        public string name { get; set; }

        public string codeRequirement { get; set; }

        public int divisionId { get; set; }

        public string divisionName { get; set; }

        public string PurchasingCOA { get; set; }
        public string StockCOA { get; set; }
        public string LocalDebtCOA { get; set; }
        public string ImportDebtCOA { get; set; }
    }
}
