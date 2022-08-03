using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using CsvHelper;
using Com.Moonlay.NetCore.Lib.Service;
using CsvHelper.Configuration;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Microsoft.EntityFrameworkCore;
using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Core.WebApi.Helpers
{
    public class BasicUploadController<TService, TModel, TViewModel, TModelMap, TDbContext> : Controller
        where TDbContext : DbContext
        where TModelMap : ClassMap
        where TModel : StandardEntity, IValidatableObject
        where TService : StandardEntityService<TDbContext, TModel>, IBasicUploadCsvService<TViewModel>, IMap<TModel, TViewModel>
    {
        private readonly TService _service;
        private readonly string ApiVersion;
        private readonly string ContentType = "application/vnd.openxmlformats";
        private readonly string FileName = string.Concat("Error Log - ", typeof(TModel).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");

        public BasicUploadController(TService service, string ApiVersion)
        {
            _service = service;
            this.ApiVersion = ApiVersion;
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var UploadedFile = Request.Form.Files[0];
                    StreamReader Reader = new StreamReader(UploadedFile.OpenReadStream());
                    List<string> FileHeader = new List<string>(Reader.ReadLine().Split(","));
                    var ValidHeader = _service.CsvHeader.SequenceEqual(FileHeader, StringComparer.OrdinalIgnoreCase);

                    if (ValidHeader)
                    {
                        Reader.DiscardBufferedData();
                        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        Reader.BaseStream.Position = 0;

                        CsvReader Csv = new CsvReader(Reader);
                        Csv.Configuration.IgnoreQuotes = false;
                        Csv.Configuration.Delimiter = ",";
                        Csv.Configuration.RegisterClassMap<TModelMap>();
                        Csv.Configuration.HeaderValidated = null;

                        List<TViewModel> Data = Csv.GetRecords<TViewModel>().ToList();
                        
                        Tuple<bool, List<object>> Validated = _service.UploadValidate(Data, Request.Form.ToList());

                        Reader.Close();

                        if (Validated.Item1) /* If Data Valid */
                        {
                            using (var Transaction = _service.DbContext.Database.BeginTransaction())
                            {
                                foreach (TViewModel modelVM in Data)
                                {
                                    TModel model = _service.MapToModel(modelVM);
                                    _service.DbSet.Add(model);
                                    _service.OnCreating(model);
                                }

                                _service.DbContext.SaveChanges();

                                Transaction.Commit();

                                Dictionary<string, object> Result =
                                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                                    .Ok();
                                return Created(HttpContext.Request.Path, Result);
                            }
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
                           new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, General.CSV_ERROR_MESSAGE)
                           .Fail();

                        return NotFound(Result);
                    }
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.NO_FILE_ERROR_MESSAGE)
                            .Fail();
                    return BadRequest(Result);
                }
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