using Com.DanLiris.Service.Core.Lib.Models;
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

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentCategoryControllerTests
{
    [Collection("TestFixture Collection")]
    public class Basic
    {
        private const string URI = "v1/master/garment-categories";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public Basic(TestServerFixture fixture)
        {
            TestFixture = fixture;

        }

        protected GarmentCategoryDataUtil DataUtil
        {
            get { return (GarmentCategoryDataUtil)this.TestFixture.Service.GetService(typeof(GarmentCategoryDataUtil)); }
        }

        public GarmentCategoryViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentCategoryViewModel()
            {
                code = guid,
                name = string.Format("TEST g-category {0}", guid),
                codeRequirement = string.Format("TEST g-category {0}", guid),
                uom=new UomViewModel
                {
                    Id=1,
                    Unit = string.Format("TEST g-category {0}", guid)
                }
            };
        }

        //[Fact]
        //public async Task Get()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task GetById()
        //{
        //    var response = await this.Client.GetAsync(string.Concat(URI, "/"));
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        [Fact]
        public async Task Post()
        {

            GarmentCategoryViewModel categoryViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(categoryViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Code()
        //{
        //    string byCodeUri = "v1/master/garment-categories/byCode";
        //    GarmentCategory Model = await DataUtil.GetTestDataAsync();

        //    var response = await this.Client.GetAsync(string.Concat(byCodeUri, "/", Model.CodeRequirement));
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = response.Content.ReadAsStringAsync().Result;
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.True(result["data"].GetType().Name.Equals("JObject"));
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Code()
        //{
        //    string byCodeUri = "v1/master/garment-categories/byCodeReq";
        //    GarmentCategory model = await DataUtil.GetTestDataAsync();
        //    var response = await this.Client.GetAsync($"{byCodeUri}/{model.Code}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}
    }
}
