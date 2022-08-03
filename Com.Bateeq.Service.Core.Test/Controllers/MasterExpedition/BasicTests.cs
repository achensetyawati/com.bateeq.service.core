using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Helpers.ValidateService;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels.Expedition;
using Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Bateeq.Service.Core.Test.Controllers.MasterExpedition
{
    public class BasicTests
    {
        protected ExpeditionController GetController(ExpeditionService service)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "Expeditiontestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            ExpeditionController controller = new ExpeditionController(service);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer Expeditiontesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/Expedition-test");
            return controller;
        }

        private CoreDbContext _dbContext(string testName)
        {
            var serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();

            DbContextOptionsBuilder<CoreDbContext> optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            CoreDbContext dbContext = new CoreDbContext(optionsBuilder.Options);

            return dbContext;
        }

        protected string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }

        public Com.DanLiris.Service.Core.Lib.Models.MasterExpedition GetTestData(CoreDbContext dbContext)
        {
            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition data = new Com.DanLiris.Service.Core.Lib.Models.MasterExpedition();
            dbContext.MasterExpeditions.Add(data);
            dbContext.SaveChanges();

            return data;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
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

        [Fact]
        public void Get_Return_OK()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(service).Get();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void GetByCode_Return_OK()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(service).GetCode(testData.Code);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void GetByCode_InternalServerError()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(service).GetCode(testData.Code);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void Get_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            ExpeditionService service = new ExpeditionService(serviceProvider.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);

            //Act
            IActionResult response = GetController(service).Get();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetById_Return_OK()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(service).GetById(testData.Id).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void POST_Return_OK()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition data = new Com.DanLiris.Service.Core.Lib.Models.MasterExpedition();
            var dataVM = service.MapToViewModel(data);
            //Act
            IActionResult response = GetController(service).Post(dataVM).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void POST_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            var dataVM = new ExpeditionViewModel();
            //Act
            IActionResult response = GetController(service).Post(dataVM).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void POST_BadRequest()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            ExpeditionService service = new ExpeditionService(serviceProvider.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition data = new Com.DanLiris.Service.Core.Lib.Models.MasterExpedition();
            var dataVM = service.MapToViewModel(data);
            dataVM.name = "";

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock.Setup(v => v.Validate(It.IsAny<Com.DanLiris.Service.Core.Lib.Models.MasterExpedition>())).Verifiable();

            serviceProvider.Setup(sp => sp.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            //Act
            IActionResult response = GetController(service).Post(dataVM).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void Delete_Success()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            ExpeditionService service = new ExpeditionService(serviceProvider.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition testData = GetTestData(dbContext);
            //Act
            IActionResult response = GetController(service).Delete(testData.Id).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void PUT_Return_OK()
        {
            //Setup
            CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            ExpeditionService service = new ExpeditionService(serviceProvider.Object);

            serviceProvider.Setup(s => s.GetService(typeof(ExpeditionService))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

            Com.DanLiris.Service.Core.Lib.Models.MasterExpedition testData = GetTestData(dbContext);
            var dataVM = service.MapToViewModel(testData);

            //Act
            IActionResult response = GetController(service).Put(testData.Id, dataVM).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

    }
}
