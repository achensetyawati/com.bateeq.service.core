using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.StandardTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class StandardTestBasicTest : BasicServiceTest<CoreDbContext, StandardTestsService, Models.StandardTests>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public StandardTestBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.StandardTests model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Remark = string.Empty;
        }

        public override void EmptyUpdateModel(Models.StandardTests model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Remark = string.Empty;
        }

        public override Models.StandardTests GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.StandardTests()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
                Remark = "remark",
            };
        }
    }
}
