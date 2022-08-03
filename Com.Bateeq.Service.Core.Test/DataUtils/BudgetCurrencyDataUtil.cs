using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
	public class BudgetCurrencyDataUtil : BasicDataUtil<CoreDbContext, BudgetCurrencyService, BudgetCurrency>, IEmptyData<BudgetCurrencyViewModel>
	{
		public BudgetCurrencyDataUtil(CoreDbContext dbContext, BudgetCurrencyService service) : base(dbContext, service)
		{
		}

		public BudgetCurrencyViewModel GetEmptyData()
		{
			BudgetCurrencyViewModel Data = new BudgetCurrencyViewModel();
            Data.rate = 1;
            
			return Data;
		}

		public override BudgetCurrency GetNewData()
		{
			BudgetCurrency model = new BudgetCurrency();

			string guid = Guid.NewGuid().ToString();

			model.Code = guid;
			model.Date = DateTime.Now;
			model.Rate = 1;
			model.Remark = guid;

			return model;
		}

        public BudgetCurrencyViewModel GetNewData2()
        {

            string guid = Guid.NewGuid().ToString();

            BudgetCurrencyViewModel TestData = new BudgetCurrencyViewModel
            {
                code = "test",
                rate = "",
                date = DateTime.Now
            };


            return TestData;
        }

        public BudgetCurrencyViewModel GetNewData3()
        {

            string guid = Guid.NewGuid().ToString();

            BudgetCurrencyViewModel TestData = new BudgetCurrencyViewModel
            {
                code = "test",
                rate = "",
                date = DateTime.Now
            };


            return TestData;
        }


        public BudgetCurrencyViewModel GetNewData4()
        {

            string guid = Guid.NewGuid().ToString();

            BudgetCurrencyViewModel TestData = new BudgetCurrencyViewModel
            {
                code = "test",
                rate = "144",
                date = DateTime.Now,
                
            };


            return TestData;
        }
        public override async Task<BudgetCurrency> GetTestDataAsync()
		{
			BudgetCurrency model = GetNewData();
			await this.Service.CreateModel(model);
			return model;
		}
	}
}
