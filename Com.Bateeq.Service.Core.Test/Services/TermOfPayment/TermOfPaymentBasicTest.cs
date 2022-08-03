using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.TermOfPaymentTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class TermOfPaymentBasicTest : BasicServiceTest<CoreDbContext, TermOfPaymentService, TermOfPayment>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public TermOfPaymentBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(TermOfPayment model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.IsExport = false;
        }

        public override void EmptyUpdateModel(TermOfPayment model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.IsExport = false;
        }

        public override TermOfPayment GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new TermOfPayment
            {
                Code = string.Format("TEST {0}", guid),
                Name = string.Format("TEST {0}", guid),
                IsExport = false,
            };
        }
    }
}
