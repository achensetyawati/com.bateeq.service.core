using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Test.DataUtils;
using System;
using System.Collections.Generic;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentCurrency
{
	[Collection("ServiceProviderFixture Collection")]
	public class GarmentCurrencyBasicTest : BasicServiceTest<CoreDbContext, GarmentCurrencyService, Models.GarmentCurrency>
	{
		private static readonly string[] createAttrAssertions = { "Code","Date","Rate" };
		private static readonly string[] updateAttrAssertions = { "Code","Date","Rate" };
		private static readonly string[] existAttrCriteria = { "Code","Date" };

		private GarmentCurrencyDataUtil DataUtil
		{
			get { return (GarmentCurrencyDataUtil)ServiceProvider.GetService(typeof(GarmentCurrencyDataUtil)); }
		}

		private GarmentCurrencyService Services
		{
			get { return (GarmentCurrencyService)ServiceProvider.GetService(typeof(GarmentCurrencyService)); }
		}

		public GarmentCurrencyBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
		{
		}

		public override void EmptyCreateModel(Models.GarmentCurrency model)
		{
			model.Code = string.Empty;
			model.Date = DateTime.MaxValue;
			model.Rate = -1;
		}

		public override void EmptyUpdateModel(Models.GarmentCurrency model)
		{
			model.Code = string.Empty;
			model.Date = DateTime.MaxValue;
			model.Rate = -1;
		}

		public override Models.GarmentCurrency GenerateTestModel()
		{
			string guid = Guid.NewGuid().ToString();

			return new Models.GarmentCurrency()
			{
				Code = guid,
				Date = DateTime.Today,
				Rate = 0,
			};
		}

		[Fact]
		public async void Should_Success_Get_Data_By_Id()
		{
			Models.GarmentCurrency model1 = await DataUtil.GetTestDataAsync();
			Models.GarmentCurrency model2 = await DataUtil.GetTestDataAsync();
			var Response = Services.GetByIds(new List<int> { model1.Id, model2.Id });
			Assert.NotNull(Response);
		}

        [Fact]
        public async void Should_Success_Get_Data_By_Code()
        {
            Models.GarmentCurrency model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByCode(model.Code);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_Single_Data_By_Code()
        {
            Models.GarmentCurrency model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetSingleByCode(model.Code);
            Assert.NotNull(Response);
        }

		[Fact]
		public async void Should_Success_Get_Single_Data_By_Code_Date()
		{
			Models.GarmentCurrency model = await DataUtil.GetTestDataAsync();
			var Response = Services.GetSingleByCodeDate(model.Code, model.Date);
			Assert.NotNull(Response);
		}
	}
}
