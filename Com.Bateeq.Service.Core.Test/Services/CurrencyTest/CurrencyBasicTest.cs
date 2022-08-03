using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.CurrencyTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class CurrencyBasicTest : BasicServiceTest<CoreDbContext, CurrencyService, Currency>
    {
        private static readonly string[] createAttrAssertions = { "Code", "Symbol", "Rate" };
        private static readonly string[] updateAttrAssertions = { "Code", "Symbol", "Rate" };
        private static readonly string[] existAttrCriteria = { "Code", "Description" };

        public CurrencyBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Currency model)
        {
            model.Code = string.Empty;
            model.Symbol = string.Empty;
            model.Rate = -1;
        }

        public override void EmptyUpdateModel(Currency model)
        {
            model.Code = string.Empty;
            model.Symbol = string.Empty;
            model.Rate = -1;
        }

        public override Currency GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Currency
            {
                Code = string.Format("CurrencyCode {0}", guid),
                Symbol = "^_^",
                Rate = 1,
                Description = string.Format("CurrencySymbol {0}", guid),
            };
        }
    }
}
