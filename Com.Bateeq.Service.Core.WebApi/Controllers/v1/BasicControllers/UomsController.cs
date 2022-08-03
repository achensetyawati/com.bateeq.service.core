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
    [Route("v{version:apiVersion}/master/uoms")]
    public class UomsController : BasicController<UomService, Uom, UomViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        UomService service;

        public UomsController(UomService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        [HttpGet("simple")]
        public IActionResult GetSimple()
        {
            try
            {
                List<Uom> Data = service.GetSimple();
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

        [HttpGet("simple-warping-weaving")]
        public IActionResult GetSimpleWarpingWeaving()
        {
            List<Uom> Data = service.GetSimpleWarpingWeaving();
            var result = Data.Select(x => service.MapToViewModel(x));
            Dictionary<string, object> Result =
                new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(result);

            return Ok(Result);
        }
    }
}
