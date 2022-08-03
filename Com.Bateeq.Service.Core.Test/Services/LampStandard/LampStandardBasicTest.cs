using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.LampStandard
{
    [Collection("ServiceProviderFixture Collection")]
    public class LampStandardBasicTest : BasicServiceTest<CoreDbContext, LampStandardService, Models.LampStandard>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public LampStandardBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.LampStandard model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Remark = string.Empty;
        }

        public override void EmptyUpdateModel(Models.LampStandard model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Remark = string.Empty;
        }

        public override Models.LampStandard GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.LampStandard()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
                Remark = "remark",
            };
        }
    }
}
