using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.Moonlay.NetCore.Lib.Service;
using Newtonsoft.Json;
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
    public class GarmentProductBasicTest : BasicServiceTest<CoreDbContext, GarmentProductService, Models.GarmentProduct>
    {
        private static readonly string[] createAttrAssertions = { "Code" };
        private static readonly string[] updateAttrAssertions = { "Code" };
        private static readonly string[] existAttrCriteria = { "Code" };

        private GarmentProductServiceDataUtil DataUtil
        {
            get { return (GarmentProductServiceDataUtil)ServiceProvider.GetService(typeof(GarmentProductServiceDataUtil)); }
        }

        private GarmentProductService Services
        {
            get { return (GarmentProductService)ServiceProvider.GetService(typeof(GarmentProductService)); }
        }

        public GarmentProductBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.GarmentProduct model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.UomUnit = string.Empty;
            model.ProductType = "FABRIC";
            model.UomId = 0;
        }

        public override void EmptyUpdateModel(Models.GarmentProduct model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.UomUnit = string.Empty;
            model.ProductType = "FABRIC";
            model.Composition = string.Empty;
            model.Const = string.Empty;
            model.Yarn = string.Empty;
            model.Width = string.Empty;
            model.UomId = 0;
        }

        public override Models.GarmentProduct GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.GarmentProduct()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
                Active = true,
                ProductType = "FABRIC",
                Composition = string.Format("TEST {0}", guid),
                Const = string.Format("TEST {0}", guid),
                Yarn = string.Format("TEST {0}", guid),
                Width = string.Format("TEST {0}", guid),
                UomId = 1,
                UomUnit = "uom",
            };
        }
        [Fact]
        public void Should_Success_Get_Data()
        {
            GarmentProduct model = DataUtil.GetNewData2();
            var Response = Services.ReadModel(1, 25, "{\"Code\":\"desc\"}", null, "", "{}");
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            GarmentProduct model1 = await DataUtil.GetTestDataAsync();
            GarmentProduct model2 = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByIds(new List<int> { model1.Id, model2.Id });
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            GarmentProduct model1 = await DataUtil.GetTestDataAsync();
            GarmentProduct model2 = DataUtil.GetNewData2();
            var Response = Services.Create(model2);
            Assert.NotEqual(Response, 0);
        }

        [Fact]
        public async Task Should_Error_Create_Data_With_Same_Combination_of_Composition()
        {
            GarmentProduct model1 = await DataUtil.GetTestDataAsync2();
            try
            {
                await DataUtil.GetTestDataAsync2();
            }
            catch (ServiceValidationExeption ex)
            {
                Assert.Equal(ex.Message, "Validation Error");
            }
        }

        // Assert.NotNull(Response);
        [SkippableFact]
        public override async Task TestCreateModel_Exist()
        {
            await Task.FromResult(1);
            Skip.If(true);
        }

        [Fact]
        public void Should_Error_Upload_CSV_Data_with_false_UOM()
        {
            GarmentProductViewModel Vmodel1 = DataUtil.GetNewData4();
            GarmentProductViewModel Vmodel2 = DataUtil.GetNewData4();
            GarmentProductViewModel Vmodel3 = DataUtil.GetNewData5();
            GarmentProductViewModel Vmodel4 = DataUtil.GetNewData5();
            var Response = Services.UploadValidate(new List<GarmentProductViewModel> { Vmodel1, Vmodel2, Vmodel3, Vmodel4 }, null);
            Assert.Equal(Response.Item1, false);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Name()
        {
            GarmentProduct model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByName(model.Name);
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task GetDistinctProductConst()
        {
            GarmentProduct model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetDistinctProductConst(model.Const, "{\"Const\":\"test\"}");
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task GetDistinctProductComposition()
        {
            GarmentProduct model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetDistinctProductComposition(model.Composition, "{\"Composition\":\"test\"}");
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task GetDistinctProductYarn()
        {
            GarmentProduct model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetDistinctProductYarn(model.Yarn, "{\"Yarn\":\"test\"}");
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task GetDistinctProductwidth()
        {
            GarmentProduct model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetDistinctProductWidth(model.Width, "{\"Width\":\"test\"}");
            Assert.NotNull(Response);
        }

    }
}
