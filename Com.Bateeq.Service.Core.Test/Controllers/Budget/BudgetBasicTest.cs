using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.Budget
{
    [Collection("TestFixture Collection")]
    public class BudgetBasicTest
    {
        private const string URI = "v1/master/budgets";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public BudgetBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
            
        }

        public BudgetViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new BudgetViewModel()
            {
                code = guid,
                name = string.Format("TEST BUDGET {0}", guid),
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

            BudgetViewModel budgetVM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(budgetVM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }    
    }
}
