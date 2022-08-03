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
    public class GarmentBuyerBrandDataUtil : BasicDataUtil<CoreDbContext, GarmentBuyerBrandService, GarmentBuyerBrand>,IEmptyData<GarmentBuyerBrandViewModel>
    {
        public GarmentBuyerBrandDataUtil(CoreDbContext dbContext, GarmentBuyerBrandService service) : base(dbContext, service)
        {
        }

        public GarmentBuyerBrandViewModel GetEmptyData()
        {
            GarmentBuyerBrandViewModel viewModel = new GarmentBuyerBrandViewModel();
            viewModel.Buyers = new GarmentBuyerViewModel {
                Id = 0,
                Code = "",
                Name=""
            };

            return viewModel;
        }
        public override GarmentBuyerBrand GetNewData()
        {
            GarmentBuyerBrand model = new GarmentBuyerBrand();

            string guid = Guid.NewGuid().ToString();

            model.Code = guid;
            model.Name = $"Name-{guid}";
            model.BuyerId = 1;
            model.BuyerName = "BUYER";
            model.BuyerCode = "BYR";
            return model;
        }
        public override async Task<GarmentBuyerBrand> GetTestDataAsync()
        {
            GarmentBuyerBrand model = GetNewData();
            await this.Service.CreateModel(model);
            return model;
        }
        public GarmentBuyerBrandViewModel GetUploadData()
        {
            GarmentBuyerBrandViewModel viewModel = new GarmentBuyerBrandViewModel();
            viewModel.Code = "brandCode";
            viewModel.Name = "brandName";
            viewModel.Buyers = new GarmentBuyerViewModel
            {
                Id = 0,
                Code = "codeBYR",
                Name = ""
            };

            return viewModel;

        }
    }
}
