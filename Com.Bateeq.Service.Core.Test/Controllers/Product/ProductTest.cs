using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.Product
{
    [Collection("TestFixture Collection")]
    public class ProductTest
    {
        private const string URI = "v1/master/products";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ProductTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public ProductViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new ProductViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Code = "Code",
                Active = true,
                Description = "desc",
                Price = 12,
                Tags = "tags",
                UOM = new ProductUomViewModel { Unit = "unit", Id = 1 },
                Currency = new ProductCurrencyViewModel { Symbol = "rp", Code = "idr", Id = 1 },
            };
        }

        protected ProductServiceDataUtil DataUtil
        {
            get { return (ProductServiceDataUtil)this.TestFixture.Service.GetService(typeof(ProductServiceDataUtil)); }
        }

        public string GeneratePackingModel()
        {
            string content = "{\"PackingDetails\":[{\"Weight\":3,\"Quantity\":2,\"Length\":4,\"Lot\":\"2\",\"Grade\":\"BS FINISH\",\"Remark\":\"1\"},{\"Weight\":11,\"Quantity\":22,\"Length\":11,\"Lot\":\"321\",\"Grade\":\"A\"}],\"DeliveryType\":\"BARU\",\"FinishedProductType\":\"WHITE\",\"OrderTypeName\":\"YARN DYED\",\"ColorName\":\"black\",\"Material\":\"MATERIAL 02\",\"MaterialWidthFinish\":\"3\",\"Date\":\"2018 - 09 - 03T17: 00:00.000Z\",\"PackingUom\":\"ROLL\",\"BuyerId\":25,\"BuyerCode\":\"A000A\",\"BuyerName\":\"ALI IMRON\",\"BuyerAddress\":\"S O L O\",\"BuyerType\":\"Lokal\",\"ProductionOrderId\":43,\"ProductionOrderNo\":\"F / 2018 / 0001\",\"OrderTypeId\":6,\"OrderTypeCode\":\"GH9YFUL5\",\"SalesContractNo\":\"0001 / FPL / 9 / 2018\",\"DesignNumber\":null,\"DesignCode\":null,\"ColorType\":null,\"MaterialId\":2,\"MaterialConstructionFinishId\":2,\"MaterialConstructionFinishName\":\"118x84\"}";
            return content;
        }

        [Fact]
        public async Task Get()
        {
            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNulTags()
        {
            var response = await this.Client.GetAsync(URI+"/null-tags");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Exception_GetNulTags()
        {
            var response = await this.Client.GetAsync(URI + "/null-tags?Select=null&Keyword=null&Filter=name");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByIdForSpinning()
        {
            var Model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync(string.Concat(URI, "/spinning/", Model.Id));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByIdForSpinning_NoFound()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/spinning/", 0));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post()
        {

            ProductViewModel productViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(productViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostPacking()
        {
            string content = GeneratePackingModel();
            var response = await this.Client.PostAsync(URI + "/packing/create", new StringContent(content, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetByProductionOrderNo()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byProductionOrderNo"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSimple()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/simple"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byId"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_GetByName()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/by-name"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
