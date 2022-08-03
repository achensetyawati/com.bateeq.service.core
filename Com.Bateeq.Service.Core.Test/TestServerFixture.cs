using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Xunit;
using Com.DanLiris.Service.Core.Lib.Services;

namespace Com.DanLiris.Service.Core.Test
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _server;
        public HttpClient Client { get; }
        public IServiceProvider Service { get; }

        public TestServerFixture()
        {
            /*
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();
            */

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    /*
                    new KeyValuePair<string, string>("Authority", "http://localhost:5000"),
                    new KeyValuePair<string, string>("ClientId", "dl-test"),
                    */
                    new KeyValuePair<string, string>("Secret", "DANLIRISTESTENVIRONMENT"),
					new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    new KeyValuePair<string, string>("DefaultConnection", "Server=localhost,1401;Database=com.danliris.db.core.controller.test;User=sa;password=Standar123.;MultipleActiveResultSets=true;")
                    //new KeyValuePair<string, string>("DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=com-danliris-db-test;Trusted_Connection=True;MultipleActiveResultSets=true"),

                })
                .Build();


            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services
                        .AddTransient<DivisionDataUtil>()
                        .AddTransient<DesignMotiveDataUtil>()
                        .AddTransient<UnitDataUtil>()
                        .AddTransient<OrderTypeDataUtil>()
                        .AddTransient<ProcessTypeDataUtil>()
                        .AddTransient<TermOfPaymentDataUtil>()
                        .AddTransient<HolidayDataUtil>()
                        .AddTransient<BuyerDataUtil>()
                        .AddTransient<BudgetServiceDataUtil>()
                        .AddTransient<MaterialConstructionServiceDataUtil>()
                        .AddTransient<QualityServiceDataUtil>()
                        .AddTransient<YarnMaterialServiceDataUtil>()
                        .AddTransient<ComodityServiceDataUtil>()
                        .AddTransient<IncomeTaxDataUtil>()
                        .AddTransient<LampStandardDataUtil>()
                        .AddTransient<StandardTestDataUtil>()
                        .AddTransient<GarmentCategoryDataUtil>()
                        .AddTransient<ProductServiceDataUtil>()
                        .AddTransient<AccountBankDataUtil>()
                        .AddTransient<GarmentProductServiceDataUtil>()
                        .AddTransient<GarmentBuyerDataUtil>()
                        .AddTransient<GarmentBuyerBrandDataUtil>()
                        .AddTransient<GarmentSupplierDataUtil>()
						.AddTransient<GarmentCurrencyDataUtil>()
						.AddTransient<BudgetCurrencyDataUtil>()
                        .AddTransient<UomServiceDataUtil>()
                        .AddTransient<MachineSpinningDataUtil>()
                        .AddTransient<ColorTypeDataUtil>()
                        .AddTransient<FinishTypeDataUtil>()
                        .AddTransient<StorageDataUtil>()
                        .AddTransient<GarmentUnitDataUtil>()
                        .AddTransient<SupplierDataUtil>()
                        .AddTransient<CurrencyDataUtil>()
                        .AddTransient<CategoryDataUtil>()
                        .AddTransient<SizeDataUtil>();
                })
                .UseStartup<Startup>();

            string authority = configuration["Authority"];
            string clientId = configuration["ClientId"];
            string secret = configuration["Secret"];

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            Service = _server.Host.Services;

            var User = new { username = "dev2", password = "Standar123" };

            HttpClient httpClient = new HttpClient();

			var response = httpClient.PostAsync("http://localhost:5000/v1/authenticate", new StringContent(JsonConvert.SerializeObject(User).ToString(), Encoding.UTF8, "application/json")).Result;

			response.EnsureSuccessStatusCode();

            var data = response.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Result.ToString());
            var token = result["data"].ToString();

            Client.SetBearerToken(token);

        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }

    [CollectionDefinition("TestFixture Collection")]
    public class TestServerFixtureCollection : ICollectionFixture<TestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}