using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Helpers.ValidateService;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.Bateeq.Service.Core.Test.DataUtils;
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

namespace Com.Bateeq.Service.Core.Test.Services.StoreTest
{
    public class StoreBasicTest
    {
        private const string ENTITY = "Stores";

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


        private StoreDataUtil _dataUtil(StoreService service)
        {
            CoreDbContext dbContext = _dbContext(GetCurrentMethod());
            return new StoreDataUtil(dbContext, service);
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
        private StorageDataUtil _dataUtilStorage(StorageService service)
        {
            CoreDbContext dbContext = _dbContext(GetCurrentMethod());
            return new StorageDataUtil(dbContext, service);
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {

            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            // var service = new StoreService(GetServiceProvider().Object);
            var data = _dataUtil(service).GetNewData();

            var Response = await service.CreateAsync(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Get_Data()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
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

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
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

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
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

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();

            var Response = await service.DeleteAsync(data.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Validate_Model()
        {
            var serviceProvider = GetServiceProvider();

            var model = new Com.DanLiris.Service.Core.Lib.Models.MasterStore();
            var validationContext = new ValidationContext(model, serviceProvider.Object, null);

            var result = model.Validate(validationContext);

            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Category()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();

            var Response = await service.GetStoreByCategory(data.StoreCategory);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_Data_Nearest()
        {
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            StoreService service = new StoreService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);
            var data = await _dataUtil(service).GetTestDataAsync();

            var Response = await service.GetNearestStoreByCode(data.Code);
            Assert.NotNull(Response);
        }

            //[Fact]
            //public async void Should_Success_Get_Data_By_Code()
            //{
            //    CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            //    Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            //    Mock<IServiceProvider> storageServiceProvider = GetServiceProvider();

            //    StoreService service = new StoreService(serviceProvider.Object);
            //    StorageService storageService = new StorageService(serviceProvider.Object);

            //    serviceProvider.Setup(s => s.GetService(typeof(StoreService))).Returns(service);
            //    serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            //    storageServiceProvider.Setup(s => s.GetService(typeof(StorageService))).Returns(storageService);
            //    storageServiceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            //    var dataStorage = await _dataUtilStorage(storageService).GetTestDataAsync();
            //    var data = _dataUtil(service).GetNewData();
            //    data.Code = dataStorage.Code;
            //    var createdData = await service.CreateAsync(data);

            //    var Response = await service.GetStoreStorageByCode(data.Code);
            //    Assert.NotNull(Response);
            //}

    }
}
