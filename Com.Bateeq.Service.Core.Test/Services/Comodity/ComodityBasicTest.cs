using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;
namespace Com.DanLiris.Service.Core.Test.Services.Comodity
{
    [Collection("ServiceProviderFixture Collection")]
    public class ComodityBasicTest : BasicServiceTest<CoreDbContext, ComodityService, Models.Comodity>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };
        public ComodityBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.Comodity model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(Models.Comodity model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override Models.Comodity GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.Comodity()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
            };
        }
    }
}
