using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.UploadControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/upload-garment-currencies")]
    public class GarmentCurrenciesUploadController : BasicUploadController<GarmentCurrencyService, GarmentCurrency, GarmentCurrencyViewModel, GarmentCurrencyService.GarmentCurrencyMap, CoreDbContext>
    {
        private static readonly string ApiVersion = "1.0";

        public GarmentCurrenciesUploadController(GarmentCurrencyService service) : base(service, ApiVersion)
        {

        }
    }
}