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

namespace Com.DanLiris.Service.Core.Test.Controllers.TermOfPaymentTest
{
    [Collection ("TestFixture Collection")]
    public class TermOfPaymentControllerTest
    {

        private const string URI = "v1/master/term-of-payments";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public TermOfPaymentControllerTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected TermOfPaymentDataUtil DataUtil
        {
            get { return (TermOfPaymentDataUtil)this.TestFixture.Service.GetService(typeof(TermOfPaymentDataUtil)); }
        }

        protected TermOfPaymentService Service
        {
            get { return (TermOfPaymentService)this.TestFixture.Service.GetService(typeof(TermOfPaymentService)); }
        }

        public TermOfPaymentViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new TermOfPaymentViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Code = string.Format("TEST {0}", guid),
                IsExport = false,

            };
        }

        [Fact]
        public async Task Post()
        {
            TermOfPaymentViewModel VM = GenerateTestModel();
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
            TermOfPayment TermOfPayment = await DataUtil.GetTestDataAsync();
            TermOfPaymentViewModel VM = Service.MapToViewModel(TermOfPayment);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            TermOfPayment TermOfPayment = await DataUtil.GetTestDataAsync();
            TermOfPaymentViewModel VM = Service.MapToViewModel(TermOfPayment);
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
