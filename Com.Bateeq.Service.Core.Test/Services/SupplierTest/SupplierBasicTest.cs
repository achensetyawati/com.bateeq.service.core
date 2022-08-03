using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.SupplierTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class SupplierBasicTest : BasicServiceTest<CoreDbContext, SupplierService, Supplier>
    {
        private static readonly string[] createAttrAssertions = { "Name", "Code" };
        private static readonly string[] updateAttrAssertions = { "Name", "Code" };
        private static readonly string[] existAttrCriteria = { "Code" };

        public SupplierBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Supplier model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(Supplier model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override Supplier GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Supplier
            {
                Code = string.Format("SupplierCode {0}", guid),
                Name = string.Format("SupplierName {0}", guid),
            };
        }
    }
}
