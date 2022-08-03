using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Xunit;
using System.Threading.Tasks;
using Com.Moonlay.NetCore.Lib.Service;
using System.Linq;
using System.Collections.Generic;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Test;

namespace Com.Danliris.Service.Core.Test.Helpers
{
    public abstract class BasicServiceTest<TDbContext, TService, TModel> : IDisposable
        where TDbContext : DbContext
        where TService : BasicService<TDbContext, TModel>
        where TModel : StandardEntity, IValidatableObject, new()
    {
        protected IServiceProvider ServiceProvider { get; set; }
        private string[] CreateAttrAssertions;
        private string[] UpdateAttrAssertions;
        private string[] ExistAttrCriteria;

        public BasicServiceTest(ServiceProviderFixture fixture, string[] createAttrAssertions, string[] updateAttrAssertions, string[] existAttrCriteria)
        {
            this.ServiceProvider = fixture.ServiceProvider;
            this.CreateAttrAssertions = createAttrAssertions;
            this.UpdateAttrAssertions = updateAttrAssertions;
            this.ExistAttrCriteria = existAttrCriteria;
        }

        protected TService Service
        {
            get { return this.ServiceProvider.GetService<TService>(); }
        }

        protected TDbContext DbContext
        {
            get { return this.ServiceProvider.GetService<TDbContext>(); }
        }

        public abstract TModel GenerateTestModel();

        public TModel GenerateSimilarTestModel(TModel model)
        {
            TModel similarTestModel = new TModel();
            foreach (string arg in this.CreateAttrAssertions)
            {
                var createAttr = model.GetType().GetProperty(arg).GetValue(model, null);
                similarTestModel.GetType().GetProperty(arg).SetValue(similarTestModel, createAttr);
            }
            foreach (string arg in this.ExistAttrCriteria)
            {
                var existAttr = model.GetType().GetProperty(arg).GetValue(model, null);
                similarTestModel.GetType().GetProperty(arg).SetValue(similarTestModel, existAttr);
            }
            return similarTestModel;
        }

        public abstract void EmptyCreateModel(TModel model);

        public abstract void EmptyUpdateModel(TModel model);

        public void AssertCreateEmpty(ServiceValidationExeption exception)
        {
            foreach (string arg in this.CreateAttrAssertions)
            {
                ValidationResult assertionInstance = exception.ValidationResults.FirstOrDefault(r => r.MemberNames.Contains(arg, StringComparer.CurrentCultureIgnoreCase));
                Assert.NotNull(assertionInstance);
            }
        }

        public void AssertCreateExist(ServiceValidationExeption exception)
        {
            foreach (string arg in this.ExistAttrCriteria)
            {
                ValidationResult assertionInstance = exception.ValidationResults.FirstOrDefault(r => r.MemberNames.Contains(arg, StringComparer.CurrentCultureIgnoreCase));
                Assert.NotNull(assertionInstance);
            }
        }

        public void AssertUpdateEmpty(ServiceValidationExeption exception)
        {
            foreach (string arg in this.UpdateAttrAssertions)
            {
                ValidationResult assertionInstance = exception.ValidationResults.FirstOrDefault(r => r.MemberNames.Contains(arg, StringComparer.CurrentCultureIgnoreCase));
                Assert.NotNull(assertionInstance);
            }
        }

        public virtual async Task<TModel> GetCreatedTestData(TService service)
        {
            TModel testModel = this.GenerateTestModel();

            int createdCount = await service.CreateModel(testModel);
            Assert.True(createdCount > 0);

            return testModel;
        }

        [Fact]
        public virtual async Task TestCreateModel()
        {
            TService service = this.Service;

            TModel createdData = await this.GetCreatedTestData(service);

            TModel data = await service.GetAsync(createdData.Id);
            Assert.NotNull(data);
        }

        [Fact]
        public virtual async Task TestCreateModel_Empty()
        {
            TService service = this.Service;
            TModel testData = this.GenerateTestModel();

            this.EmptyCreateModel(testData);

            try
            {
                await service.CreateModel(testData);
            }
            catch (ServiceValidationExeption ex)
            {
                this.AssertCreateEmpty(ex);
            }
        }

        [SkippableFact]
        public virtual async Task TestCreateModel_Exist()
        {
            Skip.If(this.ExistAttrCriteria == null || this.ExistAttrCriteria.Length == 0, "No Exist Criteria");

            TService service = this.Service;

            TModel createdData = await this.GetCreatedTestData(service);

            TModel similarTestModel = this.GenerateSimilarTestModel(createdData);

            try
            {
                await service.CreateModel(similarTestModel);
            }
            catch (ServiceValidationExeption ex)
            {
                this.AssertCreateExist(ex);
            }
        }

        [Fact]
        public virtual void TestReadModel()
        {
            TService service = this.Service;

            Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> data = service.ReadModel();
            Assert.NotNull(data);
        }

        [Fact]
        public virtual async Task TestReadModelById()
        {
            TService service = this.Service;
            TModel createdData = await this.GetCreatedTestData(service);

            TModel data = await service.ReadModelById(createdData.Id);
            Assert.NotNull(data);
        }

        [Fact]
        public virtual async Task TestUpdateModel()
        {
            TService service = this.Service;
            TModel createdData = await this.GetCreatedTestData(service);

            TModel data = await service.GetAsync(createdData.Id);
            Assert.NotNull(data);

            int updatedCount = await service.UpdateModel(data.Id, data);
            Assert.True(updatedCount > 0);
        }

        [Fact]
        public virtual async Task TestUpdateModel_Empty()
        {
            TService service = this.Service;
            TModel createdData = await this.GetCreatedTestData(service);

            TModel data = await service.GetAsync(createdData.Id);
            Assert.NotNull(data);

            this.EmptyUpdateModel(data);

            try
            {
                await service.UpdateModel(data.Id, data);
            }
            catch (ServiceValidationExeption ex)
            {
                this.AssertCreateEmpty(ex);
            }
        }

        [Fact]
        public virtual async Task TestDeleteModel()
        {
            TService service = this.Service;
            TModel createdData = await this.GetCreatedTestData(service);

            TModel data = await service.GetAsync(createdData.Id);
            Assert.NotNull(data);

            int affectedResult = await service.DeleteModel(data.Id);
            Assert.True(affectedResult > 0);

            data = await service.GetAsync(createdData.Id);
            Assert.Null(data);
        }

        public void Dispose()
        {
            this.ServiceProvider = null;
        }
    }
}