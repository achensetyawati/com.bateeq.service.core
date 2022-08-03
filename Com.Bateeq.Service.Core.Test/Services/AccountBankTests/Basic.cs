using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.AccountBankTests
{
    [Collection("ServiceProviderFixture Collection")]
    public class Basic : BasicServiceTest<CoreDbContext, AccountBankService, AccountBank>
    {
        private static readonly string[] createAttrAssertions = { "BankName", "AccountName","AccountNumber","CurrencyId" };
        private static readonly string[] updateAttrAssertions = { "BankName", "AccountName", "AccountNumber", "CurrencyId" };
        private static readonly string[] existAttrCriteria = { "BankName",  "AccountNumber" };

        public Basic(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(AccountBank model)
        {
            model.BankName = string.Empty;
            model.AccountName = string.Empty;
            model.AccountNumber = string.Empty;
            model.CurrencyId = null;
        }

        public override void EmptyUpdateModel(AccountBank model)
        {
            model.BankName = string.Empty;
            model.AccountName = string.Empty;
            model.AccountNumber = string.Empty;
            model.CurrencyId = null;
        }

        public override AccountBank GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new AccountBank()
            {
                Code = guid,
                BankCode = guid,
                AccountName = "TEST BANK" + guid,
                BankName = "TEST BANK" + guid,
                AccountNumber = "TEST BANK" + guid,
                BankAddress = "TEST BANK",
                CurrencyCode = "TEST BANK",
                CurrencyDescription = "TEST BANK",
                DivisionCode = "TEST BANK",
                DivisionName = "TEST BANK",
                Fax = "TEST BANK",
                Phone = "TEST BANK",
                CurrencySymbol = "IDR",
                CurrencyRate=1,
                SwiftCode = "TEST BANK",
                DivisionId=1,
                CurrencyId=1,
                AccountCOA = "COA"
            };
        }
    }
}