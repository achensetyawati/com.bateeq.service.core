using AutoMapper;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Helpers.ValidateService;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.MachineSpinning;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.WebApi.Utils;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.MachineSpinning
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/machine-spinnings")]
    [Authorize]
    public class MachineSpinningController : BaseController<MachineSpinningModel, MachineSpinningViewModel, IMachineSpinningService>
    {
        private readonly string ContentType = "application/vnd.openxmlformats";
        private readonly string FileName = string.Concat("Error Log - ", typeof(MachineSpinningModel).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");
        public MachineSpinningController(IIdentityService identityService, IValidateService validateService, IMachineSpinningService service, IMapper mapper) : base(identityService, validateService, service, mapper, "1.0.0")
        {

        }

        [HttpGet("blowing-unit-filtered")]
        public IActionResult GetLoaderByUnitType(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                Lib.Helpers.ReadResponse<MachineSpinningModel> read = Service.ReadNoOnly(page, size, order, select, keyword, filter);

                List<MachineSpinningViewModel> dataVM = Mapper.Map<List<MachineSpinningViewModel>>(read.Data);

                dataVM = dataVM.Where(x => x.Types.Any(y => y.Type.Equals("Blowing", StringComparison.OrdinalIgnoreCase))).ToList();

                Dictionary<string, object> Result =
                    new Utils.ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Mapper, dataVM, page, size, read.Count, dataVM.Count, read.Order, read.Selected);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Utils.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> PostCSVFileAsync()
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    VerifyUser();
                    var UploadedFile = Request.Form.Files[0];
                    StreamReader Reader = new StreamReader(UploadedFile.OpenReadStream());
                    List<string> FileHeader = new List<string>(Reader.ReadLine().Split(","));
                    var ValidHeader = Service.CsvHeader.SequenceEqual(FileHeader, StringComparer.OrdinalIgnoreCase);

                    if (ValidHeader)
                    {
                        Reader.DiscardBufferedData();
                        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        Reader.BaseStream.Position = 0;
                        CsvReader Csv = new CsvReader(Reader);
                        Csv.Configuration.IgnoreQuotes = false;
                        Csv.Configuration.Delimiter = ",";
                        Csv.Configuration.RegisterClassMap<MachineSpinningService.MachineSpinningMap>();
                        Csv.Configuration.HeaderValidated = null;

                        List<MachineSpinningCsvViewModel> Data = Csv.GetRecords<MachineSpinningCsvViewModel>().ToList();

                        Tuple<bool, List<object>> Validated = Service.UploadValidate(Data, Request.Form.ToList());

                        Reader.Close();

                        if (Validated.Item1) /* If Data Valid */
                        {
                            List<MachineSpinningModel> data = Service.MapFromCsv(Data);

                            await Service.UploadData(data);


                            Dictionary<string, object> Result =
                                new Utils.ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                                .Ok();
                            return Created(HttpContext.Request.Path, Result);

                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                                using (CsvWriter csvWriter = new CsvWriter(streamWriter))
                                {
                                    csvWriter.WriteRecords(Validated.Item2);
                                }

                                return File(memoryStream.ToArray(), ContentType, FileName);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, object> Result =
                           new Utils.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, General.CSV_ERROR_MESSAGE)
                           .Fail();

                        return NotFound(Result);
                    }
                }
                else
                {
                    Dictionary<string, object> Result =
                        new Utils.ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.NO_FILE_ERROR_MESSAGE)
                            .Fail();
                    return BadRequest(Result);
                }
            }
            catch (TypeConverterException ex)
            {
                Dictionary<string, object> Result =
                  new Utils.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, "Tahun, Delivery atau Kapasitas diisi huruf\n" + ex.Message)
                  .Fail();

                return StatusCode((int)HttpStatusCode.InternalServerError, Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                   new Utils.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                   .Fail();

                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("download")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                byte[] csvInBytes;
                var csv = Service.DownloadTemplate();

                string fileName = "Machine Spinnings Template.csv";

                csvInBytes = csv.ToArray();

                var file = File(csvInBytes, "text/csv", fileName);
                return file;
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                  new Utils.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                  .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("machine/types")]
        public IActionResult GetMachineTypes()
        {
            var result = Service.GetMachineTypes();
            return Ok(result);
        }

        [HttpGet("simple")]
        public IActionResult GetSimple()
        {
            try
            {
                List<MachineSpinningModel> result = Service.GetSimple();
                List<MachineSpinningViewModel> dataVM = Mapper.Map<List<MachineSpinningViewModel>>(result);
                Dictionary<string, object> Result =
                    new Helpers.ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result);

                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Helpers.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("spinning-filtered")]
        public IActionResult GetFilteredForSpinning(string type, string unitId)
        {
            try
            {
                List<MachineSpinningModel> result = Service.GetFilteredSpinning(type, unitId);
                if (type != null && type.Equals("Carding", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(x => x.Line, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.No, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.UomUnit, StringComparer.OrdinalIgnoreCase).ToList();
                }
                else
                {
                    result = result.OrderBy(x => x.No, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.UomUnit, StringComparer.OrdinalIgnoreCase).ToList();
                }

                List<MachineSpinningViewModel> dataVM = Mapper.Map<List<MachineSpinningViewModel>>(result);
                Dictionary<string, object> Result =
                    new Helpers.ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(dataVM);

                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Helpers.ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
