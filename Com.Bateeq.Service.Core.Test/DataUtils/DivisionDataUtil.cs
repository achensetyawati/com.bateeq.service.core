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

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class DivisionDataUtil : BasicDataUtil<CoreDbContext, DivisionService, Division>, IEmptyData<DivisionViewModel>
    {
        public DivisionDataUtil(CoreDbContext dbContext, DivisionService service) : base(dbContext, service)
        {
        }

        public DivisionViewModel GetEmptyData()
        {
            return new DivisionViewModel();
        }

        public override Division GetNewData()
        {
            string guid = Guid.NewGuid().ToString();

            return new Division()
            {
                Name = string.Format("Division {0}", guid),
                Code = string.Format("Division {0}", guid),
            };
        }

        public override async Task<Division> GetTestDataAsync()
        {
            var data = GetNewData();
            await this.Service.CreateModel(data);
            return data;
        }
    }
}
