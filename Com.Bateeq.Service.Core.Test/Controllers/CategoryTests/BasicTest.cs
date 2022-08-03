using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Test.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.CategoryTests
{
    [Collection("TestFixture Collection")]
    public class BasicTest : BasicControllerTestOldVM<CoreDbContext, CategoryService, Category, CategoryViewModel, CategoryDataUtil>
    {
        private const string URI = "v1/master/categories";

        private static List<string> CreateValidationAttributes = new List<string> { "Code", "Name" };
        private static List<string> UpdateValidationAttributes = new List<string> { "Code", "Name" };

        public BasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }

        [Fact]
        public async Task Should_Success_Get_JoinDivision_Data()
        {
            var response = await this.Client.GetAsync(URI + "/join-division");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = response.Content.ReadAsStringAsync().Result;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

            Assert.True(result.ContainsKey("apiVersion"));
            Assert.True(result.ContainsKey("message"));
            Assert.True(result.ContainsKey("data"));
            Assert.True(result["data"].GetType().Name.Equals("JArray"));
        }

        [Fact]
        public async Task Should_Exception_Get_JoinDivision_Data()
        {
            var response = await this.Client.GetAsync(URI + "/join-division?keyword=null&order=null&filter=null");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var json = response.Content.ReadAsStringAsync().Result;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

            Assert.True(result.ContainsKey("apiVersion"));
            Assert.True(result.ContainsKey("message"));
        }
    }
}
