using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class AccountBankViewModel : BasicViewModel
    {
        public string Code { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }

        public string BankAddress { get; set; }

        public string AccountName { get; set; }

        public string AccountCOA { get; set; }

        public string AccountNumber { get; set; }

        public string SwiftCode { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public DivisionViewModel Division { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
    }
}
