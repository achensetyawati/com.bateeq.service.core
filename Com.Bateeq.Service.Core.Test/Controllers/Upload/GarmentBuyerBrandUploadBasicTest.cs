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
    public class GarmentBuyerBrandUploadBasicTest
    {
        private const string URI = "v1/master/garment-buyer-brand-upload";
        private const string gbURI = "v1/master/upload-garment-buyers";

        protected TestServerFixture TestFixture{ get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public GarmentBuyerBrandUploadBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public async Task<string> postGarmentBuyer()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Kode Buyer,Nama,Alamat,Kota,Negara,NPWP,Jenis Buyer,Kontak,Tempo";
            string content = $"{guid},TEST,Alamat,Kota,Afghanistan,NPWP,Lokal,Kontak,1";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(gbURI, multiContent);
            return guid;
        }

        [Fact]
        public async Task Should_Success_Upload_CSV()
        {
            var buyer = await postGarmentBuyer();
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Kode Brand,Nama Brand,Kode Buyer";
            string content = $"{guid},Test {guid},{buyer}";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_Upload_CSV_Using_Memory_Stream()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Kode Brand,Nama Brand,Kode Buyer";
            string content1 = "AAA,TEST,AAA2";
            string content2 = "AAA,TEST,AAA2";

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
