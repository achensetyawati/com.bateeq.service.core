using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.IncomeTax
{
    [Collection("ServiceProviderFixture Collection")]
    public class IncomeTaxBasicTest : BasicServiceTest<CoreDbContext, IncomeTaxService, Models.IncomeTax>
    {
        private static readonly string[] createAttrAssertions = { "Rate", "Name" };
        private static readonly string[] updateAttrAssertions = { "Rate", "Name" };
        private static readonly string[] existAttrCriteria = { "Rate", "Name" };

        public IncomeTaxBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Models.IncomeTax model)
        {
            model.Rate = null;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(Models.IncomeTax model)
        {
            model.Rate = null;
            model.Name = string.Empty;
        }

        public override Models.IncomeTax GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.IncomeTax()
            {
                Rate = 0,
                Name = string.Format("TEST IncomeTax {0}", guid),
                Description="test"
            };
        }
    }
}