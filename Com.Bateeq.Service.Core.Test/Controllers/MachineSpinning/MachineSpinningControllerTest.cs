using AutoMapper;
using Com.DanLiris.Service.Core.Lib.AutoMapperProfiles;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.MachineSpinning;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Utils;
using Com.DanLiris.Service.Core.WebApi.Controllers.v1.MachineSpinning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.MachineSpinning
{

    public class MachineSpinningControllerTest : BaseControllerTest<MachineSpinningController, MachineSpinningModel, MachineSpinningViewModel, IMachineSpinningService>
    {
        //private const string URI = "v1/master/machine-spinnings";
        //protected TestServerFixture TestFixture { get; set; }
        //protected HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        [Fact]
        public void GetBlowingFiltered_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadNoOnly(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new ReadResponse<MachineSpinningModel>(new List<MachineSpinningModel>(), 0, new Dictionary<string, string>(), new List<string>()));
            mocks.Mapper.Setup(f => f.Map<List<MachineSpinningViewModel>>(It.IsAny<List<MachineSpinningModel>>())).Returns(ViewModels);

            int statusCode = GetStatusCode(GetController(mocks).GetLoaderByUnitType());
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetBlowingFiltered_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadNoOnly(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            int statusCode = GetStatusCode(GetController(mocks).GetLoaderByUnitType());
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void UploadFile_WithoutException_ReturnOK()
        {
            string header = "Merk Mesin, Nama, Jenis Mesin, Tahun Mesin, Kondisi Mesin, Kondisi Counter, Jumlah Delivery, Kapasitas/Hari, Satuan, Line, Unit";
            string content = "Merk, Nama, Jenis, 2018, Kondisi, Kondisi, 1, 1, AM, A, Unit";
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UploadData(It.IsAny<List<MachineSpinningModel>>())).ReturnsAsync(1);

            mocks.Service.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mocks.Service.Setup(f => f.UploadValidate(It.IsAny<List<MachineSpinningCsvViewModel>>(), It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
            MachineSpinningProfile profile = new MachineSpinningProfile();

            mocks.Mapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mocks.Mapper.Setup(x => x.Map<List<MachineSpinningModel>>(It.IsAny<List<MachineSpinningViewModel>>())).Returns(new List<MachineSpinningModel>() { Model });

            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + content)), 0, Encoding.UTF8.GetBytes(header + "\n" + content).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync();
            Assert.NotNull(response.Result);
        }

        [Fact]
        public void UploadFile_WithException_ReturnError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UploadData(It.IsAny<List<MachineSpinningModel>>())).Throws(new Exception());


            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";

            var response = controller.PostCSVFileAsync();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_FileNotFound()
        {
            string header = "Merk Mesin, Nama, Jenis Mesin, Tahun Mesin, Kondisi Mesin, Kondisi Counter, Jumlah Delivery, Kapasitas/Hari, Satuan, Line, Unit";
            string content = "Merk, Nama, Jenis, 2018, Kondisi, Kondisi, 1, 1, AM, A, Unit";
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UploadData(It.IsAny<List<MachineSpinningModel>>())).ReturnsAsync(1);

            mocks.Service.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mocks.Service.Setup(f => f.UploadValidate(It.IsAny<List<MachineSpinningCsvViewModel>>(), It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
            MachineSpinningProfile profile = new MachineSpinningProfile();

            mocks.Mapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mocks.Mapper.Setup(x => x.Map<List<MachineSpinningModel>>(It.IsAny<List<MachineSpinningViewModel>>())).Returns(new List<MachineSpinningModel>() { Model });

            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + content)), 0, Encoding.UTF8.GetBytes(header + "\n" + content).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { });

            var response = controller.PostCSVFileAsync();


            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_CSVError()
        {
            string header = "Merk Mesin, Nama, Jenis Mesin, Tahun Mesin, Kondisi Mesin, Kondisi Counter, Jumlah Delivery, Kapasitas/Hari, Satuan, Line, Unit";
            string content = "Merk, Nama, Jenis, 2018, Kondisi, Kondisi, 1, 1, AM, A, Unit";
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UploadData(It.IsAny<List<MachineSpinningModel>>())).ReturnsAsync(1);

            mocks.Service.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mocks.Service.Setup(f => f.UploadValidate(It.IsAny<List<MachineSpinningCsvViewModel>>(), It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
            MachineSpinningProfile profile = new MachineSpinningProfile();

            mocks.Mapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mocks.Mapper.Setup(x => x.Map<List<MachineSpinningModel>>(It.IsAny<List<MachineSpinningViewModel>>())).Returns(new List<MachineSpinningModel>() { Model });

            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + ", Test" + "\n" + content)), 0, Encoding.UTF8.GetBytes(header + ", Test" + "\n" + content).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync();

            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_ErrorInFile()
        {
            string header = "Merk Mesin, Nama, Jenis Mesin, Tahun Mesin, Kondisi Mesin, Kondisi Counter, Jumlah Delivery, Kapasitas/Hari, Satuan, Line, Unit";
            string content = "Merk, Nama, Jenis, 0, Kondisi, Kondisi, 0, 0, AM, A, Unit";
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UploadData(It.IsAny<List<MachineSpinningModel>>())).ReturnsAsync(1);

            mocks.Service.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mocks.Service.Setup(f => f.UploadValidate(It.IsAny<List<MachineSpinningCsvViewModel>>(), It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));
            MachineSpinningProfile profile = new MachineSpinningProfile();

            mocks.Mapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mocks.Mapper.Setup(x => x.Map<List<MachineSpinningModel>>(It.IsAny<List<MachineSpinningViewModel>>())).Returns(new List<MachineSpinningModel>() { Model });

            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + content)), 0, Encoding.UTF8.GetBytes(header + "\n" + content).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync();
            Assert.NotNull(response.Result);
        }

        [Fact]
        public void GetCSVTemplate_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DownloadTemplate()).Returns(new MemoryStream());

            var response = GetController(mocks).DownloadTemplate();
            Assert.NotNull(response);
        }

        [Fact]
        public void GetCSVTemplate_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DownloadTemplate()).Throws(new Exception());

            var response = GetController(mocks).DownloadTemplate();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Get_Machine_Types()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMachineTypes()).Returns(new List<string>());

            var response = GetController(mocks).GetMachineTypes();

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetSimple()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetSimple()).Throws(new Exception());

            var response = GetController(mocks).GetSimple();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetSpinningFiltered_Fail()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetFilteredSpinning(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            var response = GetController(mocks).GetFilteredForSpinning("", "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetSpinningFiltered_Ok()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetFilteredSpinning(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<MachineSpinningModel>() { new MachineSpinningModel() {
                Name = "Name",
                No = "1",
                UomUnit = "Unit"
            } });

            var response = GetController(mocks).GetFilteredForSpinning("", "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
    }
}
