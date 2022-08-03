using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.StandardTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class UomBasicTest : BasicServiceTest<CoreDbContext, UomService, Models.Uom>
    {
        private static readonly string[] createAttrAssertions = { "Unit" };
        private static readonly string[] updateAttrAssertions = { "Unit" };
        private static readonly string[] existAttrCriteria = { "Unit" };

        public UomBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.Uom model)
        {
            model.Unit = string.Empty;
        }
        public override void EmptyUpdateModel(Models.Uom model)
        {
            model.Unit = string.Empty;
        }
        public override Models.Uom GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.Uom()
            {
                Unit = string.Format("TEST {0}", guid),
            };
        }

        // [SkippableFact]
        // public override async Task TestCreateModel_Exist()
        // {
        //     Skip.If(true);
        // }

        [Fact]
        public void TestSimple()
        {
            var data = Service.GetSimple();
            Assert.NotNull(data);
        }

        [Fact]
        public void TestSimpleWarpingWeaving()
        {
            var data = Service.GetSimpleWarpingWeaving();
            Assert.NotNull(data);
        }
    }
}
