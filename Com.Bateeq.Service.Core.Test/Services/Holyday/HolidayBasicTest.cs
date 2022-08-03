using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.Holyday
{
    [Collection("ServiceProviderFixture Collection")]
    public class HolidayBasicTest : BasicServiceTest<CoreDbContext, HolidayService, Holiday>
    {
        private static readonly string[] createAttrAssertions = { "date", "name", "division", "description" };
        private static readonly string[] updateAttrAssertions = { "date", "name", "division", "description" };
        private static readonly string[] existAttrCriteria = { };
        public HolidayBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Holiday model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.DivisionId = null;
            model.Description = string.Empty;
            model.Date = null;
            model.DivisionName = string.Empty;
        }

        public override void EmptyUpdateModel(Holiday model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.DivisionId = null;
            model.Description = string.Empty;
            model.Date = null;
            model.DivisionName = string.Empty;
        }

        public override Holiday GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Holiday()
            {
                Code = guid,
                Date = DateTime.Now,
                Name = string.Format("TEST {0}", guid),
                DivisionId = 1,
                Description = string.Format("TEST {0}", guid),
                DivisionName = string.Format("TEST {0}", guid)
            };
        }

        public override void TestReadModel()
        {
            base.TestReadModel();
        }
    }
}
