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

namespace Com.DanLiris.Service.Core.Test.Controllers.StandardTestTest
{
    [Collection("TestFixture Collection")]
    public class StandardTestTest
    {
        private const string URI = "v1/master/standard-tests";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public StandardTestTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected StandardTestsService Service
        {
            get { return (StandardTestsService)this.TestFixture.Service.GetService(typeof(StandardTestsService)); }
        }

        protected StandardTestDataUtil DataUtil
        {
            get { return (StandardTestDataUtil)this.TestFixture.Service.GetService(typeof(StandardTestDataUtil)); }
        }

        public StandardTestsViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new StandardTestsViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Remark = "test"
            };
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
        public async Task Post()
        {

            StandardTestsViewModel standardTestsViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(standardTestsViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Delete()
        {
            StandardTests standard = await DataUtil.GetTestDataAsync();
            StandardTestsViewModel VM = Service.MapToViewModel(standard);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            StandardTests standard = await DataUtil.GetTestDataAsync();
            StandardTestsViewModel VM = Service.MapToViewModel(standard);
            var response = await this.Client.PutAsync(string.Concat(URI, "/", VM.Id), new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
