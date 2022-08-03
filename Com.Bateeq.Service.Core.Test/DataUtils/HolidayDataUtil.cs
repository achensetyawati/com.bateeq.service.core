using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class HolidayDataUtil : BasicDataUtil<CoreDbContext, HolidayService, Holiday>, IEmptyData<HolidayViewModel>
    {
        public HolidayDataUtil(CoreDbContext dbContext, HolidayService Service) : base(dbContext, Service)
        {
        }

        public HolidayViewModel GetEmptyData()
        {
            return new HolidayViewModel();
        }

        public override Holiday GetNewData()
        {
            string guid = Guid.NewGuid().ToString();

            return new Holiday()
            {
                Code = string.Format("TEST {0}", guid),
                Name = string.Format("TEST {0}", guid),
                Description = "Description",
                DivisionId = 0,
                DivisionName = "Division Name",
                Date = DateTime.Now,
            };
        }

        public override async Task<Holiday> GetTestDataAsync()
        {
            var data = GetNewData();
            await Service.CreateModel(data);
            return data;
        }
    }
}
