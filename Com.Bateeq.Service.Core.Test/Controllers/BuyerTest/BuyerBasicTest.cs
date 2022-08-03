using Com.DanLiris.Service.Core.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Controllers.BuyerTest
{
    [Collection("TestFixture Collection")]
    public class BuyerBasicTest
    {
        private const string URI = "v1/master/buyers";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public BuyerBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected BuyerDataUtil DataUtil
        {
            get { return (BuyerDataUtil)this.TestFixture.Service.GetService(typeof(BuyerDataUtil)); }
        }

        protected BuyerService Service
        {
            get { return (BuyerService)this.TestFixture.Service.GetService(typeof(BuyerService)); }
        }


        public BuyerViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new BuyerViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Id = 0,
                UId = "UId",
                Active = true,
                Address = "Address",
                Code = string.Format("TEST {0}", guid),
                City = "City",
                Contact = "Contact",
                Country = "Country",
                Tempo = 1,
                Type = "Type",
                NPWP ="NPWP",
            };
        }

        [Fact]
        public async Task Post()
        {
            BuyerViewModel VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            BuyerViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Bad_Request()
        {
            BuyerViewModel VM = GenerateTestModel();
            VM.Code = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete()
        {
            Buyer buyer = await DataUtil.GetTestDataAsync();
            BuyerViewModel VM = Service.MapToViewModel(buyer);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            Buyer buyer = await DataUtil.GetTestDataAsync();
            BuyerViewModel VM = Service.MapToViewModel(buyer);
            var response = await this.Client.PutAsync(string.Concat(URI, "/", VM.Id), new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task NotFound()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/", 0));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
