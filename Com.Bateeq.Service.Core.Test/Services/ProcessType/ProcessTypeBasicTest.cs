using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.ProcessTypeTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class ProcessTypeBasicTest : BasicServiceTest<CoreDbContext, ProcessTypeService, ProcessType>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public ProcessTypeBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(ProcessType model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(ProcessType model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override ProcessType GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new ProcessType()
            {
                Name = string.Format("TEST {0}", guid),
                Code = string.Format("TEST {0}", guid),
            };
        }
    }
}
