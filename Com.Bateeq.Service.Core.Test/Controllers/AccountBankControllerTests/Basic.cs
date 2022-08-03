using Com.DanLiris.Service.Core.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.AccountBankControllerTests
{
    [Collection("TestFixture Collection")]
    public class Basic
    {
        private const string URI = "v1/master/account-banks";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public Basic(TestServerFixture fixture)
        {
            TestFixture = fixture;

        }

        public AccountBankViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new AccountBankViewModel()
            {
                Code = guid,
                BankCode = guid,
                BankName = "TestBank",
                BankAddress = "TestBank",
                AccountName = "TestBank",
                AccountNumber = "TestBank",
                Fax = "TestBank",
                Phone = "TestBank",
                SwiftCode = "TestBank",
                Division= new DivisionViewModel
                {
                    Name = "DivisionName",
                    Code= "DivisionCode",
                    Id=1
                },
                Currency= new CurrencyViewModel
                {
                    Code = "IDR",
                    Id=1,
                    Description = string.Format("TEST Bank {0}", guid),
                    Rate=1,
                    Symbol = "IDR",
                },
                
            };
        }

        [Fact]
        public async Task Get()
        {
            AccountBankViewModel bankVM = GenerateTestModel();
            var post = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(bankVM).ToString(), Encoding.UTF8, "application/json"));

            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //[Fact]
        //public async Task GetById()
        //{
        //    AccountBankViewModel bankVM = GenerateTestModel();
        //    var post = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(bankVM).ToString(), Encoding.UTF8, "application/json"));

        //    var response = await this.Client.GetAsync(string.Concat(URI, "/"));
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        [Fact]
        public async Task Post()
        {

            AccountBankViewModel bankVM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(bankVM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
