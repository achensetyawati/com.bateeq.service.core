using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.StandardTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class ProductBasicTest : BasicServiceTest<CoreDbContext, ProductService, Models.Product>
    {
        private static readonly string[] createAttrAssertions = { "Code" };
        private static readonly string[] updateAttrAssertions = { "Code" };
        private static readonly string[] existAttrCriteria = { "Code" };

        public ProductBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.Product model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.CurrencyCode = string.Empty;
            model.CurrencySymbol = string.Empty;
            model.Description = string.Empty;
            model.Tags = string.Empty;
            model.UomUnit = string.Empty;
            model.Price = 0;
            model.CurrencyId = 0;
            model.UomId = 0;
            model.SPPProperties = null;
        }

        public override void EmptyUpdateModel(Models.Product model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.CurrencyCode = string.Empty;
            model.CurrencySymbol = string.Empty;
            model.Description = string.Empty;
            model.Tags = string.Empty;
            model.UomUnit = string.Empty;
            model.Price = 0;
            model.CurrencyId = 0;
            model.UomId = 0;
            model.SPPProperties = null;
        }

        public override Models.Product GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.Product()
            {
                SPPProperties = new Models.ProductSPPProperty()
                {
                    ProductionOrderNo = "ProductionOrderNo",
                    BuyerAddress = "buyerAddress",
                    BuyerName = "bname",
                    BuyerId = 1,
                    ColorName = "color",
                    Construction = "cons",
                    DesignCode = "dcode",
                    DesignNumber = "dnum",
                    Grade = "grade",
                    Length = 1,
                    Lot = "lot",
                    OrderTypeCode = "otc",
                    OrderTypeId = 1,
                    OrderTypeName = "otn",
                    ProductionOrderId = 1,

                },
                Code = string.Format("Code {0}", guid),
                Name = string.Format("TEST {0}", guid),
                CurrencySymbol = "curr",
                Active = true,
                CurrencyCode = "currcode",
                CurrencyId = 1,
                Description = "desc",
                Price = 12,
                Tags = "tags",
                UomId = 1,
                UomUnit = "uom",

            };
        }

        [SkippableFact]
        public override async Task TestCreateModel_Exist()
        {
            await Task.FromResult(1);
            Skip.If(true);
        }

        [Fact]
        public void TestSimple()
        {

            var data = Service.GetSimple();
            Assert.NotNull(data);
        }

        [Fact]
        public void TestReadModelNullTags()
        {

            Tuple<List<Models.Product>, int, Dictionary<string, string>, List<string>> data = Service.ReadModelNullTags();
            Assert.NotNull(data);
        }

        [Fact]
        public async Task TestGetForSpinning()
        {
            var createdData = await this.GetCreatedTestData(Service);
            var data = await Service.GetProductForSpinning(createdData.Id);
            Assert.NotNull(data);
        }
    }
}
