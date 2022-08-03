using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.DesignMotiveTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class DesignMotiveBasicTest : BasicServiceTest<CoreDbContext, DesignMotiveService, DesignMotive>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Code" };

        public DesignMotiveBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(DesignMotive model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(DesignMotive model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override DesignMotive GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new DesignMotive()
            {
                Code = string.Format("DesignCode {0}", guid),
                Name = string.Format("DesignName {0}", guid),
            };
        }
    }
}
