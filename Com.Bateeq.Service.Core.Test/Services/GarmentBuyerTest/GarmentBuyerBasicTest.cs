using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using Xunit;
namespace Com.DanLiris.Service.Core.Test.Services.GarmentBuyerTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class GarmentBuyerBasicTest : BasicServiceTest<CoreDbContext, GarmentBuyerService, GarmentBuyer>
    {
        private static readonly string[] createAttrAssertions = { "Name", "Code", "Country", "Type", "Tempo" };
        private static readonly string[] updateAttrAssertions = { "Name", "Code", "Country", "Type", "Tempo" };
        private static readonly string[] existAttrCriteria = { "Code" };
        public GarmentBuyerBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(GarmentBuyer model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Country = string.Empty;
            model.Type = string.Empty;
            model.Tempo = -1;
        }

        public override void EmptyUpdateModel(GarmentBuyer model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Country = string.Empty;
            model.Type = string.Empty;
            model.Tempo = -1;
        }

        public override GarmentBuyer GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentBuyer()
            {
                Code = string.Format("BuyerCode {0}", guid),
                Name = string.Format("BuyerName {0}", guid),
                Country = string.Format("BuyerCountry {0}", guid),
                Type = string.Format("BuyerType {0}", guid),
            };
        }
    }
}
