using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels.Expedition;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/expedition-service-routers/all")]
    public class ExpeditionController : BasicController<ExpeditionService, MasterExpedition, ExpeditionViewModel, CoreDbContext>
    {
        //private readonly ExpeditionService service;
        private new static readonly string ApiVersion = "1.0";
        ExpeditionService service;
        public ExpeditionController(ExpeditionService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        [HttpGet("code")]
        public IActionResult GetCode(string code)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                ExpeditionViewModel Data = service.GetbyCode(code);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);

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
