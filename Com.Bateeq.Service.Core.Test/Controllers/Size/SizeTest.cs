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

namespace Com.DanLiris.Service.Core.Test.Controllers.Size
{
    [Collection("TestFixture Collection")]
    public class SizeTest
    {
        private const string URI = "v1/master/sizes";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public SizeTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public SizeViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new SizeViewModel()
            {
                Size = string.Format("TEST {0}", guid),
            };
        }
        protected ProductServiceDataUtil DataUtil
        {
            get { return (ProductServiceDataUtil)this.TestFixture.Service.GetService(typeof(ProductServiceDataUtil)); }
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

            SizeViewModel sizeViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(sizeViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}