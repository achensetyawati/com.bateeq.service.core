
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Helpers.ValidateService;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.ItemsController
{
    [Collection("TestFixture Collection")]
    public class ItemControllerTest
    {
        private const string URI = "v1/items/finished-goods";
        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ItemControllerTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected ItemService Service
        {
            get { return (ItemService)this.TestFixture.Service.GetService(typeof(ItemService)); }
        }

        public ItemViewModelUsername GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new ItemViewModelUsername()
            {
                dataDestination = new List<ItemViewModelRead>()
                {
                    new ItemViewModelRead()
                    {
                        ArticleRealizationOrder = "123",
                        code = "21000123",
                        name = "name",
                        Size = "S",
                        Uom = "PCS",
                        ImagePath = "/sales",
                        ImgFile = "",
                        Tags = "",
                        Remark = "",
                        Description = "",
                    },
                },
                color = new ItemArticleColorViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                process = new ItemArticleProcesViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                materials = new ItemArticleMaterialViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                materialCompositions = new ItemArticleMaterialCompositionViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                collections = new ItemArticleCollectionViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                seasons = new ItemArticleSeasonViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                counters = new ItemArticleCounterViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                subCounters = new ItemArticleSubCounterViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                categories = new ItemArticleCategoryViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                DomesticCOGS = 1000,
                DomesticRetail = 0,
                DomesticSale = 10000,
                DomesticWholesale = 0,
                InternationalCOGS = 0,
                InternationalWholesale = 0,
                InternationalRetail = 0,
                InternationalSale = 0,
                ImageFile = "",
                _id = 0,
                Username = "username",
                Token = "token"
            };
        }
        
        public ItemViewModelUsername GenerateTestModelForUpdate()
        {
            string guid = Guid.NewGuid().ToString();

            return new ItemViewModelUsername()
            {
                dataDestination = new List<ItemViewModelRead>()
                {
                    new ItemViewModelRead()
                    {
                        ArticleRealizationOrder = "123",
                        code = "21000123",
                        name = "name",
                        Size = "S",
                        Uom = "PCS",
                        ImagePath = "/sales",
                        ImgFile = "",
                        Tags = "",
                        Remark = "",
                        Description = "",
                    },
                },
                color = new ItemArticleColorViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                process = new ItemArticleProcesViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                materials = new ItemArticleMaterialViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                materialCompositions = new ItemArticleMaterialCompositionViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                collections = new ItemArticleCollectionViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                seasons = new ItemArticleSeasonViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                counters = new ItemArticleCounterViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                subCounters = new ItemArticleSubCounterViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                categories = new ItemArticleCategoryViewModel()
                {
                    _id = 1,
                    code = "code",
                    name = "name"
                },
                DomesticCOGS = 1000,
                DomesticRetail = 0,
                DomesticSale = 10000,
                DomesticWholesale = 0,
                InternationalCOGS = 0,
                InternationalWholesale = 0,
                InternationalRetail = 0,
                InternationalSale = 0,
                ImageFile = "",
                _id = 1,
                Username = "username",
                Token = "token"
            };
        }

        [Fact]
        public async Task Post()
        {
            ItemViewModelUsername VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI+"/item", new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        
        [Fact]
        public async Task UpdateTotalQuantity()
        {
            ItemViewModelUsername VM = GenerateTestModelForUpdate();
            var uri = $"{URI}/update-qty-by-id/{VM._id}";
            var response = await this.Client.PutAsync(uri, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        
        [Fact]
        public async Task ReduceTotalQuantity()
        {
            ItemViewModelUsername VM = GenerateTestModel();
            var uri = $"{URI}/reduce-qty-by-id/{VM._id}";
            var response = await this.Client.PutAsync(uri, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        /*
        protected Item Model
        {
            get { return new Item(); }
        }

        protected ItemViewModel ViewModel
        {
            get { return new ItemViewModel(); }
        }

        protected List<Item> Models
        {
            get { return new List<Item>(); }
        }

        protected List<ItemViewModel> ViewModels
        {
            get { return new List<ItemViewModel>(); }
        }

        protected ServiceValidationException GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IService>(), Mapper: new Mock<IMapper>());
        }

        protected WebApi.Controllers.v1.BasicControllers.ItemsController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            WebApi.Controllers.v1.BasicControllers.ItemsController controller = (WebApi.Controllers.v1.BasicControllers.ItemsController)Activator.CreateInstance(typeof(WebApi.Controllers.v1.BasicControllers.ItemsController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object, mocks.Mapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private async Task<int> GetStatusCodePost((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IService> Service, Mock<IMapper> Mapper) mocks)
        {
            WebApi.Controllers.v1.BasicControllers.ItemsController controller = GetController(mocks);
            IActionResult response = await controller.Post(ViewModel);

            return GetStatusCode(response);
        }
        
        [Fact]
        public async Task Post_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(s => s.CreateAsync(It.IsAny<Item>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }
        */
    }
}