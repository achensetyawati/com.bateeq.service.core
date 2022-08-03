using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/items/finished-goods")]
    public class ItemsController : BasicController<ItemService, Item, ItemViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        ItemService service;

        public ItemsController(ItemService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        //[HttpGet("ro/{ro}")]
        //public IActionResult GetRo(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")]List<string> Select = null, string Keyword = "", string Filter = "{}")
        //{
        //    try
        //    {
        //        Tuple<List<Item>, int, Dictionary<string, string>, List<string>> Data = Service.GetRo(Page, Size, Order, Select, Keyword, Filter);

        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
        //            .Ok<Item, ItemViewModel>(Data.Item1, Service.MapToViewModel, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

        //        return Ok(Result);
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}


        //[HttpGet("ro/{ro}")]
        //public async Task<IActionResult> GetByRo([FromRoute]string RO)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var model = Service.GetRO1(RO);

        //        if (model == null)
        //        {
        //            Dictionary<string, object> ResultNotFound =
        //                new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
        //                .Fail();
        //            return NotFound(ResultNotFound);
        //        }

        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
        //            .Ok<Item, ItemViewModel>(model, Service.MapToViewModel);
        //        return Ok(Result);
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}

        [HttpGet("ro/{ro}")]
        public async Task<IActionResult> GetRO([FromRoute] string ro)
        {
            try
            {

               // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<ItemViewModel> Data = await service.GetRO1(ro);

                

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
        [HttpGet("ro-discount/{ro}")]
        public async Task<IActionResult> GetROForDiscount([FromRoute] string ro)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<ItemLoader> Data = await service.GetRO2(ro);



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
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetCode([FromRoute] string code)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<ItemViewModel> Data = await service.GetCode(code);

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

        [HttpGet("byCode/{code}")]
        public IActionResult GetByCode([FromRoute] string code)
        {
            try
            {
                ItemViewModel Data = service.GetByCode(code);
                //List<ItemViewModel> Data = service.GetCode(code);

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

        [HttpGet("readAll/{image}")]
        public async Task<IActionResult> GetImage([FromRoute] string image)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<ItemViewModel> Data = await service.GetImage(image);

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
        [HttpGet("readinven")]
        public async Task<IActionResult> GetDataForInven(string ro, string code, string name)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                ItemViewModel Data = service.GetDataInven(ro, code, name);

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
        [HttpGet("code-discount/{code}")]
        public async Task<IActionResult> GetDiscountCode([FromRoute] string code)
        {
            try
            {

                // service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<ItemViewModelRead> Data = await service.GetCode2(code);

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


        [HttpPut("upload/image")]
        public async Task<IActionResult> ImgPost([FromBody]ItemViewModel viewModel)
        {
            try
            {
              

                //Service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                //Service.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                //if (Request.Form.Files.Count > 0)
                //{
                    var body = await service.ImgPost(viewModel);


                    Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                    return Created(String.Concat(HttpContext.Request.Path, "/", 1), Result);
               // }
                //else {

                //    return Ok();
                //}
                    
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("post-binary")]
        public async Task<IActionResult> PostBinary()
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var body = await sr.ReadToEndAsync();
                return Ok(body);
            }
        }

        [HttpPost("item")]
        public async Task<IActionResult> PostItem([FromBody] ItemViewModelUsername ViewModel)
        {
            try
            {
                Item model = Service.ItemMapToModel(ViewModel);

                Service.Username = ViewModel.Username;
                Service.Token = ViewModel.Token;

                await Service.CreateModel(model);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(HttpContext.Request.Path, "/", model.Id), Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("update-qty-by-id/{_id}")]
        public async Task<IActionResult> UpdateTotalQty([FromRoute] int _id, [FromBody] ItemViewModelUsername ViewModel)
        {
            try
            {
                Service.Username = ViewModel.Username;
                Service.Token = ViewModel.Token;

                await Service.UpdateTotalQtyAsync(_id, ViewModel.TotalQty, ViewModel.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("reduce-qty-by-id/{_id}")]
        public async Task<IActionResult> ReduceTotalQty([FromRoute] int _id, [FromBody] ItemViewModelUsername ViewModel)
        {
            try
            {
                Service.Username = ViewModel.Username;
                Service.Token = ViewModel.Token;

                await Service.ReduceTotalQtyAsync(_id, ViewModel.TotalQty, ViewModel.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return NoContent();
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
