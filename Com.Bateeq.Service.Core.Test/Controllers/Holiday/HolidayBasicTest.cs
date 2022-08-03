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

namespace Com.DanLiris.Service.Core.Test.Controllers.HolidayBasicTest
{
    [Collection("TestFixture Collection")]
    public class HolidayBasicTest
    {
        private const string URI = "v1/master/holidays";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public HolidayBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected HolidayDataUtil DataUtil
        {
            get { return (HolidayDataUtil)this.TestFixture.Service.GetService(typeof(HolidayDataUtil)); }
        }

        protected HolidayService Service
        {
            get { return (HolidayService)this.TestFixture.Service.GetService(typeof(HolidayService)); }
        }

        public HolidayViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new HolidayViewModel()
            {
                name = string.Format("TEST {0}", guid),
                code = string.Format("TEST {0}", guid),
                date = DateTime.Now,
                description = "description",
                
            };
        }

        //[Fact]
        //public async Task Post()
        //{
        //    HolidayViewModel VM = GenerateTestModel();
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
            Holiday holiday = await DataUtil.GetTestDataAsync();
            HolidayViewModel VM = Service.MapToViewModel(holiday);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM._id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            Holiday holiday = await DataUtil.GetTestDataAsync();
            HolidayViewModel VM = Service.MapToViewModel(holiday);
            var response = await this.Client.PutAsync(string.Concat(URI, "/", VM._id), new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

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
