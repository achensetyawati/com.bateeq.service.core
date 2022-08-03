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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentBuyerBrandTest
{
    [Collection("TestFixture Collection")]
    public class BasicTests : BasicControllerTest<CoreDbContext, GarmentBuyerBrandService, GarmentBuyerBrand, GarmentBuyerBrandViewModel, GarmentBuyerBrandDataUtil>
    {
        private static string URI = "v1/master/garment-buyer-brands";
        private static List<string> CreateValidationAttributes = new List<string> { };
        private static List<string> UpdateValidationAttributes = new List<string> { };

        public BasicTests(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }

        [Fact]
        public async Task Should_Succes_Get_By_Name()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byname?", $"Keyword={Model.Name}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Exception_Get_By_Name()
        {
            var response = await this.Client.GetAsync(URI + "/byname?Keyword=null&filter=name");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
