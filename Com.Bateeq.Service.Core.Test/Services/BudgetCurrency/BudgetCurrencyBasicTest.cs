using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.BudgetCurrency
{
    [Collection("ServiceProviderFixture Collection")]
    public class BudgetCurrencyBasicTest : BasicServiceTest<CoreDbContext, BudgetCurrencyService, Models.BudgetCurrency>
    {
        private static readonly string[] createAttrAssertions = { "Code", "Date", "Rate" };
        private static readonly string[] updateAttrAssertions = { "Code", "Date", "Rate" };
        private static readonly string[] existAttrCriteria = { "Code", "Date" };

        public BudgetCurrencyBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Models.BudgetCurrency model)
        {
            model.Code = "Test";
            model.Date = DateTime.Now;
            model.Rate = 1;
        }

        public override void EmptyUpdateModel(Models.BudgetCurrency model)
        {
            model.Code = "Test";
            model.Date = DateTime.Now;
            model.Rate = 1;
        }

        public override Models.BudgetCurrency GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.BudgetCurrency()
            {
                Code = guid,
                Date = DateTime.Now,
                Rate = 1,
                Remark = guid
            };
        }

        private BudgetCurrencyDataUtil DataUtil
        {
            get { return (BudgetCurrencyDataUtil)ServiceProvider.GetService(typeof(BudgetCurrencyDataUtil)); }
        }

        private BudgetCurrencyService Services
        {
            get { return (BudgetCurrencyService)ServiceProvider.GetService(typeof(BudgetCurrencyService)); }
        }

        [Fact]
        public async void GetByTags()
        {
            Models.BudgetCurrency model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByCode(model.Code);
            Assert.NotNull(Response);
        }


        [Fact]
        public void Should_Error_Upload_CSV_Data_DuplicateCode()
        {
            BudgetCurrencyViewModel Vmodel5 = DataUtil.GetNewData2();
            BudgetCurrencyViewModel Vmodel6 = DataUtil.GetNewData3();
            var Response = Services.UploadValidate(new List<BudgetCurrencyViewModel> { Vmodel5, Vmodel6 }, new List<KeyValuePair<string, StringValues>> { });
            Assert.Equal(Response.Item1, false);
        }

        //[Fact]
        //public async void Should_Success_Upload_CSV_Data_when_UseTax_False()
        //{
        //    BudgetCurrencyViewModel Vmodel6 = await DataUtil.GetNewData4();

        //    const string DATE_KEYWORD = "date";
        //    DateTime Date;
        //    List<KeyValuePair<string, StringValues>> Body = null;
        //    string DateString = Body.SingleOrDefault(s => s.Key.Equals(DATE_KEYWORD)).Value;
        //    bool ValidDate = DateTime.TryParseExact(DateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);
        //    var Response = Services.UploadValidate(new List<BudgetCurrencyViewModel> { Vmodel6 }, new List<KeyValuePair<string, StringValues>> { });
        //    Assert.Equal(Response.Item1, true);
        //}
    }
}
