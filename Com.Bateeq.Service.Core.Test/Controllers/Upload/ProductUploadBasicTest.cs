using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.Upload
{
    [Collection("TestFixture Collection")]
    public class ProductUploadBasicTest
    {
        private const string URI = "v1/master/upload-products";
        private const string uomURI = "v1/master/upload-uoms";
        private const string currencyURI = "v1/master/upload-currencies";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ProductUploadBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public async Task<string> uploadUom()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Unit";
            string content = $"{guid}";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(uomURI, multiContent);
            return guid;
        }

        public async Task<string> uploadCurrency()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Kode,Simbol,Rate,Keterangan";
            string content = $"{guid},RP,1,Ini adalah {guid}";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(currencyURI, multiContent);
            return guid;
        }

        [Fact]
        public async Task Should_Success_Upload_CSV()
        {
            var uom = await uploadUom();
            var currency = await uploadCurrency();
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Kode Barang,Nama Barang,Satuan,Mata Uang,Harga,Tags,Keterangan";
            string content = $"{guid},{guid},{uom},{currency},1,Tags,Keterangan";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_Upload_CSV_Using_Memory_Stream()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string header = "Kode Barang,Nama Barang,Satuan,Mata Uang,Harga,Tags,Keterangan";
            string content1 = "AAA,Nama Barang,MT,Mata Uang,Harga,Tags,Keterangan";
            string content2 = "AAA,Nama Barang,MT,Mata Uang,Harga,Tags,Keterangan";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content1 + "\n" + content2);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Not_Found_Upload_CSV()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            var payload = Encoding.UTF8.GetBytes("Hello");
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Should_Internal_Server_Error()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            var payload = Encoding.UTF8.GetBytes("");
            multiContent.Add(new ByteArrayContent(payload), "file", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Should_Bad_Request()
        {
            var content = new FormUrlEncodedContent(new[]
            {
             new KeyValuePair<string, string>("", "")
            });

            var response = await Client.PostAsync(URI, content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
