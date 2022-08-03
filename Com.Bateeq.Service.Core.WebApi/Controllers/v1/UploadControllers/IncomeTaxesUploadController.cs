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
    [Route("v{version:apiVersion}/master/upload-income-taxes")]
    public class IncomeTaxesUploadController : BasicUploadController<IncomeTaxService, IncomeTax, IncomeTaxViewModel, IncomeTaxService.IncomeTaxMap, CoreDbContext>
    {
        private static readonly string ApiVersion = "1.0";

        public IncomeTaxesUploadController(IncomeTaxService service) : base(service, ApiVersion)
        {
        }
    }
}