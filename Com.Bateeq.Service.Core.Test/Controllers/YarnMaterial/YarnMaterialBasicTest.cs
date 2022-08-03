using Com.DanLiris.Service.Core.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.YarnMaterial
{
    [Collection("TestFixture Collection")]
    public class YarnMaterialBasicTest
    {
        private const string URI = "v1/master/yarn-materials";
        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public YarnMaterialBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public YarnMaterialViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new YarnMaterialViewModel()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
                Remark = "test remark",
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

            YarnMaterialViewModel VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
