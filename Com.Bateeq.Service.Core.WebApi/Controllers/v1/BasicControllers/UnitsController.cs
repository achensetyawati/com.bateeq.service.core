using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/units")]
    public class UnitsController : BasicController<UnitService, Unit, UnitViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        private readonly UnitService service;
        public UnitsController(UnitService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        [HttpGet("simple")]
        public IActionResult GetSimple()
        {
            try
            {

                List<Unit> Data = service.GetSimple();
                var result = Data.Select(x => service.MapToViewModel(x));
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result);

                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
