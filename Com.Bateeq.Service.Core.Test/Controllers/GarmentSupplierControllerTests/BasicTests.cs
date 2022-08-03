using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Models = Com.DanLiris.Service.Core.Lib.Models;
using Xunit;
using Com.DanLiris.Service.Core.Test.DataUtils;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentSupplierControllerTests
{
    [Collection("TestFixture Collection")]
    public class BasicTests : BasicControllerTest<CoreDbContext, GarmentSupplierService, Models.GarmentSupplier, GarmentSupplierViewModel, GarmentSupplierDataUtil>
	{
		private const string URI = "v1/master/garment-suppliers";
		
		private static List<string> CreateValidationAttributes = new List<string> { };
		private static List<string> UpdateValidationAttributes = new List<string> { };

		public BasicTests(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
		{
		}

	}
}

