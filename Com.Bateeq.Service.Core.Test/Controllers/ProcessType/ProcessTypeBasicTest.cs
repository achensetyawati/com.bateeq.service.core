using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.ProcessTypeTest
{
    [Collection("TestFixture Collection")]
    public class ProcessTypeBasicTest
    {
        private const string URI = "v1/master/process-types";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ProcessTypeBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected ProcessTypeDataUtil DataUtil
        {
            get { return (ProcessTypeDataUtil)this.TestFixture.Service.GetService(typeof(ProcessTypeDataUtil)); }
        }

        protected ProcessTypeService Service
        {
            get { return (ProcessTypeService)this.TestFixture.Service.GetService(typeof(ProcessTypeService)); }
        }

        public ProcessTypeViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new ProcessTypeViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Code = string.Format("TEST {0}", guid),
                Remark = "REMARK",
            };
        }

        //[Fact]
        //public async Task Post()
        //{
        //    ProcessTypeViewModel VM = GenerateTestModel();
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}

        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            BuyerViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
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
            ProcessType ProcessType = await DataUtil.GetTestDataAsync();
            ProcessTypeViewModel VM = Service.MapToViewModel(ProcessType);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            ProcessType ProcessType = await DataUtil.GetTestDataAsync();
            ProcessTypeViewModel VM = Service.MapToViewModel(ProcessType);
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
