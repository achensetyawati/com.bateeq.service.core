using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Helpers.ValidateService;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
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

namespace Com.DanLiris.Service.Core.Test.Services.UnitTest
{
    public class UnitBasicTest
    {
        private const string ENTITY = "Units";

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


        private UnitDataUtil _dataUtil(UnitService service)
        {
            CoreDbContext dbContext = _dbContext(GetCurrentMethod());
            return new UnitDataUtil(dbContext, service);
        }

        Mock<IServiceProvider> GetServiceProvider()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IIdentityService)))
              .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            var validateService = new Mock<IValidateService>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IValidateService)))
              .Returns(validateService.Object);
            return serviceProvider;
        }

        protected string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }

        [Fact]
        public async void Should_Success_Create_Data()
        {

            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            // var service = new UnitService(GetServiceProvider().Object);
            var data = _dataUtil(service).GetNewData();

            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Get_Data()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            var data = await _dataUtil(service).GetTestDataAsync();

            var Response = service.ReadModel(1, 25, "{}", null, data.Name, "{}");
            Assert.NotEmpty(Response.Item1);

            Dictionary<string, string> order = new Dictionary<string, string>()
            {
                {"Code", "asc" }
            };
            var response2 = service.ReadModel(1, 25, JsonConvert.SerializeObject(order), null, data.Name, "{}");
            Assert.NotEmpty(response2.Item1);

            Dictionary<string, string> order1 = new Dictionary<string, string>()
            {
                {"Code", "desc" }
            };
            var response3 = service.ReadModel(1, 25, JsonConvert.SerializeObject(order1), null, data.Name, "{}");
            Assert.NotEmpty(response3.Item1);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();

            var Response = await service.ReadModelById(data.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Update_Data()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();
            var newData = await service.ReadModelById(data.Id);

            var Response = await service.UpdateAsync(newData.Id, newData);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();

            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Validate_Model()
        {
            var serviceProvider = GetServiceProvider();

            var model = new Com.DanLiris.Service.Core.Lib.Models.Unit();
            var validationContext = new ValidationContext(model, serviceProvider.Object, null);

            var result = model.Validate(validationContext);

            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public async void Validate_ViewModel_Duplicate()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            var data = await _dataUtil(service).GetTestDataAsync();
            var viewModel = new UnitViewModel
            {
                Code = data.Code,
                Division=new DivisionViewModel()
                {
                    Name="name",
                    Id=1,
                    Code="code"
                }
            };

            var model = service.MapToModel(viewModel);
            var validationContext = new ValidationContext(model, serviceProvider.Object, null);
            var result = model.Validate(validationContext);

            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public async void Should_Fail_Upload_Validate_Data()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();
            var viewModel = service.MapToViewModel(data);

            List<UnitViewModel> units = new List<UnitViewModel>() { new UnitViewModel(){ Division= new DivisionViewModel()} };
            var Response = service.UploadValidate(units, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public async void Should_Fail_Upload_Validate_Double_Uploaded_Data()
        {

            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            UnitService service = new UnitService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(UnitService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();
            var viewModel = service.MapToViewModel(data);
            var viewModel2 = service.MapToViewModel(data);

            List<UnitViewModel> units = new List<UnitViewModel>() { viewModel, viewModel2 };
            var Response = service.UploadValidate(units, null);
            Assert.False(Response.Item1);
        }
    }
}
