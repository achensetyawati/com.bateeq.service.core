using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.MachineSpinning;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.MachineSpinning
{
    public class MachineSpinningServiceTest
    {
        private const string ENTITY = "JournalTransaction";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private CoreDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<CoreDbContext> optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            CoreDbContext dbContext = new CoreDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private MachineSpinningDataUtil _dataUtil(MachineSpinningService service)
        {
            return new MachineSpinningDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }

        [Fact]
        public async void Should_Success_Get_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData(_dbContext(GetCurrentMethod()));
            var Response = service.Read(1, 25, "{}", null, data.Name, "{}");
            Assert.NotEmpty(Response.Data);

            Dictionary<string, string> order = new Dictionary<string, string>()
            {
                {"No", "asc" }
            };
            var response2 = service.Read(1, 25, JsonConvert.SerializeObject(order), null, data.Name, "{}");
            Assert.NotEmpty(response2.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData(_dbContext(GetCurrentMethod()));
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = _dataUtil(service).GetNewData(_dbContext(GetCurrentMethod()));
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_No_Error_Validate_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToValidate(_dbContext(GetCurrentMethod()));
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(CoreDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));
            ValidationContext validationDuplicateContext = new ValidationContext(vm, serviceProvider.Object, null);
            Assert.True(vm.Validate(validationDuplicateContext).Count() == 0);
        }

        [Fact]
        public async void Should_No_Error_Validate_Data_Duplicate_Name()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = _dataUtil(service).GetNewData(_dbContext(GetCurrentMethod()));
            var Response = await service.CreateAsync(model);
            var vm = _dataUtil(service).GetDataToValidate(_dbContext(GetCurrentMethod()));
            vm.Name = model.Name;
            vm.No = model.No;
            vm.UnitName = model.UnitName;
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(CoreDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));
            ValidationContext validationDuplicateContext = new ValidationContext(vm, serviceProvider.Object, null);
            Assert.True(vm.Validate(validationDuplicateContext).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            var vm = new MachineSpinningViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Duplicate_Type_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToValidate(_dbContext(GetCurrentMethod()));
            var typeResponse = service.GetMachineTypes();
            vm.Types = new List<MachineSpinningProcessTypeViewModel>()
            {
                new MachineSpinningProcessTypeViewModel()
                {
                    Type = typeResponse.First()
                },
                new MachineSpinningProcessTypeViewModel()
                {
                    Type = typeResponse.First()
                }
            };
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(CoreDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));
            ValidationContext validationDuplicateContext = new ValidationContext(vm, serviceProvider.Object, null);
            Assert.True(vm.Validate(validationDuplicateContext).Count() > 0);
        }

        [Fact]
        public void Should_No_Error_Validate_CSV_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(CoreDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));
            ValidationContext validationDuplicateContext = new ValidationContext(vm, serviceProvider.Object, null);
            Assert.True(vm.Validate(validationDuplicateContext).Count() == 0);
        }

        [Fact]
        public async void Should_No_Error_Validate_CSV_Data_Duplicate_Name()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = _dataUtil(service).GetNewData(_dbContext(GetCurrentMethod()));
            var Response = await service.CreateAsync(model);
            var vm = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));
            vm.Name = model.Name;
            vm.No = model.No;
            vm.UnitName = model.UnitName;
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(CoreDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));
            ValidationContext validationDuplicateContext = new ValidationContext(vm, serviceProvider.Object, null);
            Assert.True(vm.Validate(validationDuplicateContext).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_CSV_Data()
        {
            var vm = new MachineSpinningCsvViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        

        [Fact]
        public async void Should_Success_Update_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData(_dbContext(GetCurrentMethod()));
            var newModel = await service.ReadByIdAsync(model.Id);
            var Response = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(service).GetTestData(_dbContext(GetCurrentMethod()));
            //var modelToDelete = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(model.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Success_Get_CSV()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var reportResponse = service.DownloadTemplate();
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public void Should_Success_Map_From_CSV()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var vm = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));
            var models = service.MapFromCsv(new List<MachineSpinningCsvViewModel>() { vm });
            Assert.NotNull(models.Count > 0);
        }

        [Fact]
        public void Should_Success_Get_MachineTypes()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var reportResponse = service.GetMachineTypes();
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public async void Should_Success_Upload_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = _dataUtil(service).GetNewData(_dbContext(GetCurrentMethod()));

            List<MachineSpinningModel> machineSpinnings = new List<MachineSpinningModel>() { model };

            var result = await service.UploadData(machineSpinnings);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public void Should_Success_Upload_Validate_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var viewModel = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));

            List<MachineSpinningCsvViewModel> machineSpinnings = new List<MachineSpinningCsvViewModel>() { viewModel };
            var Response = service.UploadValidate(machineSpinnings, null);
            Assert.True(Response.Item1);
        }

        [Fact]
        public void Should_Fail_Upload_Validate_Empty_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            List<MachineSpinningCsvViewModel> machineSpinnings = new List<MachineSpinningCsvViewModel>() { new MachineSpinningCsvViewModel() };
            var Response = service.UploadValidate(machineSpinnings, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public void Should_Fail_Upload_Validate_Double_Uploaded_Data()
        {
            
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var viewModel = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));
            var viewModel2 = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));

            List<MachineSpinningCsvViewModel> machineSpinnings = new List<MachineSpinningCsvViewModel>() { viewModel, viewModel2 };
            var Response = service.UploadValidate(machineSpinnings, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public async void Should_Fail_Upload_Validate_Existed_Data()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var viewModel = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));

            List<MachineSpinningCsvViewModel> machineSpinningsVM = new List<MachineSpinningCsvViewModel>() { viewModel };

            var model = _dataUtil(service).GetNewData(_dbContext(GetCurrentMethod()));
            model.No = viewModel.No;
            List<MachineSpinningModel> machineSpinningsModel = new List<MachineSpinningModel>() { model };
            await service.UploadData(machineSpinningsModel);
            var Response2 = service.UploadValidate(machineSpinningsVM, null);
            Assert.False(Response2.Item1);
        }

        [Fact]
        public void Should_Fail_Upload_Validate_NotExist_Type()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var viewModel = _dataUtil(service).GetDataToUpload(_dbContext(GetCurrentMethod()));
            viewModel.Type = "aaaaa";
            List<MachineSpinningCsvViewModel> machineSpinnings = new List<MachineSpinningCsvViewModel>() { viewModel };
            var Response = service.UploadValidate(machineSpinnings, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public void TestSimple()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = service.GetSimple();
            Assert.NotNull(Response);
        }

        [Fact]
        public void TestSpinningFiltered()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = service.GetFilteredSpinning("","");
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_DataByNo()
        {
            var service = new MachineSpinningService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData(_dbContext(GetCurrentMethod()));
            var Response = service.ReadNoOnly(1, 25, "{}", null, data.No, "{}");
            Assert.NotEmpty(Response.Data);
        }
    }
}
