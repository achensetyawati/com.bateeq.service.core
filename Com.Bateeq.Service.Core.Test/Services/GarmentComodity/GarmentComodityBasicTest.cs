using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentComodity
{
    [Collection("ServiceProviderFixture Collection")]
    public class GarmentComodityBasicTest : BasicServiceTest<CoreDbContext, GarmentComodityService, Models.GarmentComodity>
    {
        private static readonly string[] createAttrAssertions = { "Code", "Name" };
        private static readonly string[] updateAttrAssertions = { "Code", "Name" };
        private static readonly string[] existAttrCriteria = { "Code" };
        public GarmentComodityBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.GarmentComodity model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(Models.GarmentComodity model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override Models.GarmentComodity GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.GarmentComodity()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
            };
        }
    }
}
