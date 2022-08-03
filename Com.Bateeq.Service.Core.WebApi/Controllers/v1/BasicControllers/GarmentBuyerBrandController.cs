using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{ 

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/garment-buyer-brands")]
    public class GarmentBuyerBrandController : BasicController<GarmentBuyerBrandService, GarmentBuyerBrand, GarmentBuyerBrandViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        GarmentBuyerBrandService service;
        public GarmentBuyerBrandController(GarmentBuyerBrandService service) : base(service, ApiVersion)
        {
            this.service = service;
        }
        [HttpGet("byName")]
        public IActionResult GetByName(string Keyword = "", string filter = "{}")
        {
            try
            {

                service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IQueryable< GarmentBuyerBrand> Data = service.GetByName(Keyword,filter);

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