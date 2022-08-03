using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Core.Test.DataUtils
{
    public class StoreDataUtil : BasicDataUtil<CoreDbContext, StoreService, MasterStore>, IEmptyData<StoreViewModel>
    {
        public StoreDataUtil(CoreDbContext dbContext, StoreService service) : base(dbContext, service)
        {
        }

        public StoreViewModel GetEmptyData()
        {
            return new StoreViewModel();
        }

        public override MasterStore GetNewData()
        {
            string guid = Guid.NewGuid().ToString();

            return new MasterStore()
            {
                Name = string.Format("StorageName {0}", guid),
                Code = "code",
                StoreCategory="cat",
                City="city"
            };
        }

        public override async Task<MasterStore> GetTestDataAsync()
        {
            var data = GetNewData();
            await this.Service.CreateModel(data);
            return data;
        }

        //public async Task<MasterStore> GetTestDataAsyncWithStorage()
        //{
        //    var storage = await Task.Run(() => StorageDataUtil.GetTestDataAsync());
        //    var data = GetNewData();
        //    data.Code = storage.Code;
        //    await this.Service.CreateModel(data);
        //    return data;
        //}
    }
}
