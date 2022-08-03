using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;
using System.Threading.Tasks;
using System;
using System.Net;

using System.Collections.Generic;


namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/storages")]
    public class StoragesController : BasicController<StorageService, Storage, StorageViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";

        StorageService service;

        public StoragesController(StorageService service) : base(service, ApiVersion)
        {
            this.service = service;

        }

        [HttpGet("by-storage-name")]
        public async Task<IActionResult> GetByStorageName(string keyword, int page = 1, int size = 25)
        {
            var result = await Service.GetStorageByName(keyword, page, size);

            return Ok(result);
        }

        [HttpGet("{storageId}")]
        public async Task<IActionResult> GetRO([FromRoute] string storageId)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<StorageViewModel> Data = await service.GetStorage1(storageId);

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

        [HttpGet("destination")]
        public IActionResult GetDestination(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")]List<string> Select = null, string Keyword = "", string Filter = "{}")
        {
            //try
            //{
            //    Tuple<List<Storage>, int, Dictionary<string, string>, List<string>> Data = service.ReadModel(Page, Size, Order, Select, Keyword, Filter);

            //    Dictionary<string, object> Result =
            //        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
            //        .Ok<Storage, Storage>(Data.Item1, service.MapToViewModel, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

            //    return Ok(Result);
            //}


            try
            {

                var data = service.GetDestination(Page, Size, Order, Select, Keyword, Filter);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("source")]
        public IActionResult GetSource(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")]List<string> Select = null, string Keyword = "", string Filter = "{}")
        {

            try
            {

                var data = service.GetSource(Page, Size, Order, Select, Keyword, Filter);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
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
        public IActionResult GetSource(string code)
        {

            try
            {

                var data = service.GetStoragebyCode(code);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    info = new { total = 1 },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
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
