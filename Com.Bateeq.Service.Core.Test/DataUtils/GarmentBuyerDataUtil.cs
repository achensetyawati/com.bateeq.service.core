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
    public class GarmentBuyerDataUtil : BasicDataUtil<CoreDbContext, GarmentBuyerService, GarmentBuyer>, IEmptyData<GarmentBuyerViewModel>
    {
        public GarmentBuyerDataUtil(CoreDbContext dbContext, GarmentBuyerService service) : base(dbContext, service)
        {
        }

        public GarmentBuyerViewModel GetEmptyData()
        {
            GarmentBuyerViewModel viewModel = new GarmentBuyerViewModel();

            return viewModel;
        }

        public override GarmentBuyer GetNewData()
        {
            GarmentBuyer model = new GarmentBuyer();

            string guid = Guid.NewGuid().ToString();

            model.Code = guid;
            model.Name = $"Name-{guid}";
            model.Address = $"Address-{guid}";
            model.City = $"City-{guid}";
            model.Country = $"Country-{guid}";
            model.Contact = $"Contact-{guid}";
            model.Tempo = 0;
            model.Type = $"Type-{guid}";
            model.NPWP = $"NPWP-{guid}";

            return model;
        }

        public override async Task<GarmentBuyer> GetTestDataAsync()
        {
            GarmentBuyer model = GetNewData();
            await this.Service.CreateModel(model);
            return model;
        }
    }
}
