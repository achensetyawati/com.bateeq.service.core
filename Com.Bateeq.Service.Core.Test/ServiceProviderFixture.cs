using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Services.MachineSpinning;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Com.DanLiris.Service.Core.Test
{
    public class ServiceProviderFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public ServiceProviderFixture()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Secret", "DANLIRISTESTENVIRONMENT"),
					new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    new KeyValuePair<string, string>("DefaultConnection",  "Server=localhost,1401; Database = com.danliris.db.core.service.test; User = sa; password = Standar123.; MultipleActiveResultSets = true; ")
                    //new KeyValuePair<string, string>("DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=com-danliris-db-test;Trusted_Connection=True;MultipleActiveResultSets=true"),
                   
                })
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["DefaultConnection"];

            this.ServiceProvider = new ServiceCollection()
                .AddDbContext<CoreDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(connectionString);
                }, ServiceLifetime.Transient)
                .AddTransient<UnitService>(provider => new UnitService(provider))
                .AddTransient<UnitDataUtil>()
                .AddTransient<OrderTypeService>(provider => new OrderTypeService(provider) { Username = "TEST" })
                .AddTransient<OrderTypeDataUtil>()
                .AddTransient<ProcessTypeService>(provider => new ProcessTypeService(provider))
                .AddTransient<ProcessTypeDataUtil>()
                .AddTransient<TermOfPaymentService>(provider => new TermOfPaymentService(provider))
                .AddTransient<TermOfPaymentDataUtil>()
                .AddTransient<HolidayService>(provider => new HolidayService(provider))
                .AddTransient<HolidayDataUtil>()
                .AddTransient<DesignMotiveService>(provider => new DesignMotiveService(provider))
                .AddTransient<DesignMotiveDataUtil>()
                .AddTransient<BudgetService>(provider => new BudgetService(provider))
                .AddTransient<BudgetServiceDataUtil>()
                .AddTransient<ComodityService>(provider => new ComodityService(provider))
                .AddTransient<ComodityServiceDataUtil>()
                .AddTransient<QualityService>(provider => new QualityService(provider))
                .AddTransient<QualityServiceDataUtil>()
                .AddTransient<YarnMaterialService>(provider => new YarnMaterialService(provider))
                .AddTransient<YarnMaterialServiceDataUtil>()
                .AddTransient<MaterialConstructionService>(provider => new MaterialConstructionService(provider))
                .AddTransient<MaterialConstructionServiceDataUtil>()
                .AddTransient<IncomeTaxService>(provider => new IncomeTaxService(provider))
                .AddTransient<IncomeTaxDataUtil>()
                .AddTransient<LampStandardService>(provider => new LampStandardService(provider))
                .AddTransient<LampStandardDataUtil>()
                .AddTransient<StandardTestsService>(provider => new StandardTestsService(provider))
                .AddTransient<StandardTestDataUtil>()
                .AddTransient<DivisionService>(provider => new DivisionService(provider))
                .AddTransient<DivisionDataUtil>()
                .AddTransient<ProductService>(provider => new ProductService(provider))
                .AddTransient<ProductServiceDataUtil>()
                .AddTransient<AccountBankDataUtil>()
                .AddTransient<AccountBankService>(provider => new AccountBankService(provider))
                .AddTransient<GarmentProductService>(provider => new GarmentProductService(provider))
                .AddTransient<GarmentProductServiceDataUtil>()
                .AddTransient<GarmentCategoryDataUtil>()
                .AddTransient<GarmentCategoryService>(provider => new GarmentCategoryService(provider))
                .AddTransient<GarmentBuyerService>(provider => new GarmentBuyerService(provider))
                .AddTransient<GarmentComodityService>(provider => new GarmentComodityService(provider))
                .AddTransient<GarmentSectionService>(provider => new GarmentSectionService(provider))
                .AddTransient<StandardMinuteValueService>(provider => new StandardMinuteValueService(provider))
                .AddTransient<GarmentSupplierDataUtil>()
                .AddTransient<GarmentBuyerBrandDataUtil>()
                .AddTransient<GarmentBuyerDataUtil>()
                .AddTransient<GarmentSupplierService>(provider => new GarmentSupplierService(provider))
				.AddTransient<GarmentUnitService>(provider => new GarmentUnitService(provider))
                .AddTransient<GarmentBuyerBrandService>(provider => new GarmentBuyerBrandService(provider))
                .AddTransient<UnitService>(provider => new UnitService(provider))
				.AddTransient<GarmentCurrencyService>(provider => new GarmentCurrencyService(provider))
				.AddTransient<GarmentCurrencyDataUtil>()
				.AddTransient<BudgetCurrencyService>(provider => new BudgetCurrencyService(provider))
				.AddTransient<BudgetCurrencyDataUtil>()
                .AddTransient<UomService>(provider => new UomService(provider))
                .AddTransient<UomServiceDataUtil>()
                .AddTransient<MachineSpinningService>(provider => new MachineSpinningService(provider))
                .AddTransient<MachineSpinningDataUtil>()
                .AddTransient<SizeService>(provider => new SizeService(provider))
                .AddTransient<SizeDataUtil>()
                //.AddTransient<AccountRoleDataUtil>()
                //.AddTransient<PermissionDataUtil>()
                .AddTransient(provider => new StorageService(provider))
                .AddTransient(provider => new BuyerService(provider))
                .AddTransient(provider => new CategoryService(provider))
                .AddTransient(provider => new CurrencyService(provider))
                .AddTransient(provider => new SupplierService(provider))

                .BuildServiceProvider();

            CoreDbContext dbContext = ServiceProvider.GetService<CoreDbContext>();
            dbContext.Database.Migrate();
        }

        public void Dispose()
        {
        }
    }

    [CollectionDefinition("ServiceProviderFixture Collection")]
    public class ServiceProviderFixtureCollection : ICollectionFixture<ServiceProviderFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}