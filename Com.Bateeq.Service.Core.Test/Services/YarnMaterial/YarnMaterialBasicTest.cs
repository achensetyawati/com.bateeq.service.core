using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.YarnMaterial
{
    [Collection("ServiceProviderFixture Collection")]
    public class YarnMaterialBasicTest : BasicServiceTest<CoreDbContext, YarnMaterialService, Models.YarnMaterial>
    {
        //private static readonly string[] createAttrAssertions = { "Code", "Name" };
        //private static readonly string[] updateAttrAssertions = { "Code", "Name" };
        //private static readonly string[] existAttrCriteria = { "Code" };

        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public YarnMaterialBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.YarnMaterial model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Remark = string.Empty;
        }

        public override void EmptyUpdateModel(Models.YarnMaterial model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Remark = string.Empty;
        }

        public override Models.YarnMaterial GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.YarnMaterial()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
                Remark = "remark",
            };
        }
    }
}
