using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels.Expedition;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Core.Test.DataUtils
{
    public class MasterExpeditionDataUtil : BasicDataUtil<CoreDbContext, ExpeditionService, MasterExpedition>, IEmptyData<ExpeditionViewModel>
    {
        public MasterExpeditionDataUtil(CoreDbContext dbContext, ExpeditionService service) : base(dbContext, service)
        {
        }

        public ExpeditionViewModel GetEmptyData()
        {
            return new ExpeditionViewModel();
        }

        public override MasterExpedition GetNewData()
        {
            string guid = Guid.NewGuid().ToString();

            return new MasterExpedition()
            {
                Name = string.Format("ExpeditionName {0}", guid),
                Code = string.Format("ExpeditionName {0}", guid),

            };
        }

        public override async Task<MasterExpedition> GetTestDataAsync()
        {
            var data = GetNewData();
            await this.Service.CreateModel(data);
            return data;
        }
    }
}
