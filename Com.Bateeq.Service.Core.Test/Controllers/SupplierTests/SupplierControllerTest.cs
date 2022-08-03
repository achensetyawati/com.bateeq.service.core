using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Test.Helpers;
using System.Collections.Generic;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.SupplierTests
{
    [Collection("TestFixture Collection")]
    public class SupplierControllerTest : BasicControllerTestOldVM<CoreDbContext, SupplierService, Supplier, SupplierViewModel, SupplierDataUtil>
    {
        private const string URI = "v1/master/suppliers";

        private static List<string> CreateValidationAttributes = new List<string> { };
        private static List<string> UpdateValidationAttributes = new List<string> { };

        public SupplierControllerTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }
    }
}
