using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.DesignMotiveTest
{
    [Collection("TestFixture Collection")]
    public class DesignMotiveBasicTest
    {
        private const string URI = "v1/master/design-motives";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public DesignMotiveBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected DesignMotiveDataUtil DataUtil
        {
            get { return (DesignMotiveDataUtil)this.TestFixture.Service.GetService(typeof(DesignMotiveDataUtil)); }
        }

        protected DesignMotiveService Service
        {
            get { return (DesignMotiveService)this.TestFixture.Service.GetService(typeof(DesignMotiveService)); }
        }


        public DesignMotiveViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new DesignMotiveViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Code = string.Format("TEST {0}", guid),
            };
        }

        [Fact]
        public async Task Post()
        {
            DesignMotiveViewModel VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            DesignMotiveViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Bad_Request()
        {
            DesignMotiveViewModel VM = GenerateTestModel();
            VM.Name = null;
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
            DesignMotive motive = await DataUtil.GetTestDataAsync();
            DesignMotiveViewModel VM = Service.MapToViewModel(motive);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            DesignMotive motive = await DataUtil.GetTestDataAsync();
            DesignMotiveViewModel VM = Service.MapToViewModel(motive);
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
