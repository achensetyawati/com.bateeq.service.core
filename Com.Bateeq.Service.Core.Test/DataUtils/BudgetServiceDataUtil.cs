using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class BudgetServiceDataUtil : BasicDataUtil<CoreDbContext, BudgetService, Budget>, IEmptyData<BudgetViewModel>
    {

        public BudgetServiceDataUtil(CoreDbContext dbContext, BudgetService service) : base(dbContext, service)
        {
        }        

        public BudgetViewModel GetEmptyData()
        {
            BudgetViewModel Data = new BudgetViewModel();

            Data.name = "";
            Data.code = "";
            return Data;
        }

        public override Budget GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            Budget TestData = new Budget
            {
                Name = "TEST",
                Code = guid
            };

            return TestData;
        }

        public override async Task<Budget> GetTestDataAsync()
        {
            Budget Data = GetNewData();
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}