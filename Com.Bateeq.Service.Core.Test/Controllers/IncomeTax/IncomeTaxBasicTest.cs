using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using System.Collections.Generic;
using Models = Com.DanLiris.Service.Core.Lib.Models;
using Xunit;
using Com.DanLiris.Service.Core.Test.DataUtils;

namespace Com.DanLiris.Service.Core.Test.Controllers.IncomeTax
{
    [Collection("TestFixture Collection")]
    public class IncomeTaxBasicTest : BasicControllerTest<CoreDbContext, IncomeTaxService, Models.IncomeTax, IncomeTaxViewModel, IncomeTaxDataUtil>
    {
        private const string URI = "v1/master/income-taxes";

        private static List<string> CreateValidationAttributes = new List<string> { };
        private static List<string> UpdateValidationAttributes = new List<string> { };

        public IncomeTaxBasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }
    }
}
