using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
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
    [Route("v{version:apiVersion}/master/stores")]
    public class StoreController : BasicController<StoreService, MasterStore, StoreViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        StoreService service;
        public StoreController(StoreService service) : base(service, ApiVersion)
        {
            this.service = service;
        }
        [HttpGet("category")]
        public async Task<IActionResult> GetRO(string category)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<MasterStore> Data = await service.GetStoreByCategory(category);



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

        [HttpGet("code")]
        public async Task<IActionResult> GetbyCode(string code)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                MasterStore Data = await service.GetStoreByCode(code);



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
        [HttpGet("store-storage")]
        public async Task<IActionResult> GetStoreStoragebyCode(string code)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                StoreStorageViewModel Data = await service.GetStoreStorageByCode(code);



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

        [HttpGet("nearest-store")]
        public async Task<IActionResult> GetNearest(string code)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<MasterStore> Data = await service.GetNearestStoreByCode(code);

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
