using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class AccountBankDataUtil : BasicDataUtil<CoreDbContext, AccountBankService, AccountBank>, IEmptyData<AccountBankViewModel>
    {

        public AccountBankDataUtil(CoreDbContext dbContext, AccountBankService service) : base(dbContext, service)
        {
        }

        public AccountBankViewModel GetEmptyData()
        {
            AccountBankViewModel Data = new AccountBankViewModel();

            Data.AccountName = "";
            Data.BankAddress = "";
            Data.Code = "";
            Data.Currency = null;
            Data.Division = null;
            Data.AccountNumber = "";
            Data.BankName = "";
            Data.SwiftCode = "";
            Data.Phone = "";
            Data.Fax = "";
            return Data;
        }

        public override AccountBank GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            AccountBank TestData = new AccountBank
            {
                BankName = "TEST",
                BankAddress = "TEST",
                SwiftCode = "TEST",
                Phone = "TEST",
                DivisionCode = "TEST",
                DivisionId = 1,
                DivisionName = "TEST",
                CurrencyCode = "TEST",
                CurrencyDescription = "TEST",
                CurrencyId = 1,
                CurrencyRate=2,
                CurrencySymbol = "TEST",
                Fax = "TEST",
                AccountName = "TEST",
                AccountNumber = "TEST",
                Code = guid
            };

            return TestData;
        }

        public override async Task<AccountBank> GetTestDataAsync()
        {
            AccountBank Data = GetNewData();
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}