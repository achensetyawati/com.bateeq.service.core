using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/holidays")]
    public class HolidaysController : BasicController<HolidayService, Holiday, HolidayViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";

        public HolidaysController(HolidayService service) : base(service, ApiVersion)
        {
        }
    }
}
