using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
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

namespace Com.DanLiris.Service.Core.Test.Controllers.UomTest
{
    [Collection("TestFixture Collection")]
    public class UomTest
    {
        private const string URI = "v1/master/uoms";
        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public UomTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected UomServiceDataUtil DataUtil
        {
            get { return (UomServiceDataUtil)this.TestFixture.Service.GetService(typeof(UomServiceDataUtil)); }
        }

        protected UomService Service
        {
            get { return (UomService)this.TestFixture.Service.GetService(typeof(UomService)); }
        }

        public UomViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new UomViewModel()
            {
                Unit = string.Format("TEST {0}", guid),
            };
        }

        [Fact]
        public async Task Post()
        {
            UomViewModel VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            UomViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Bad_Request()
        {
            UomViewModel VM = GenerateTestModel();
            VM.Unit = null;
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
            Uom Uom = await DataUtil.GetTestDataAsync();
            UomViewModel VM = Service.MapToViewModel(Uom);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            Uom Uom = await DataUtil.GetTestDataAsync();
            UomViewModel VM = Service.MapToViewModel(Uom);
            var response = await this.Client.PutAsync(string.Concat(URI, "/", VM.Id), new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task NotFound()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/", 0));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSimple()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/simple"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSimpleWarpingWeaving()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/simple-warping-weaving"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
