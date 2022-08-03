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
    public class GarmentProductUploadBasicTest
    {
        private const string URI = "v1/master/upload-garmentProducts";
        private const string uomURI = "v1/master/upload-uoms";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public GarmentProductUploadBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public async Task<string> uploadUOM()
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

        [Fact]
        public async Task Should_Success_Upload_CSV()
        {
            var uom = await uploadUOM();
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Jenis Produk,Kode Barang,Nama Barang,Satuan,Komposisi,Const,Yarn,Width,Tags,Keterangan";
            string content = $"FABRIC,{guid},FABRIC,{uom},{guid},Const,Yarn,Widht,Tags,Keterangan";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_Upload_CSV_Using_Memory_Stream()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string header = "Jenis Produk,Kode Barang,Nama Barang,Satuan,Komposisi,Const,Yarn,Width,Tags,Keterangan";
            string content1 = "FABRIC,FA,FABRIC,MT,Komposisi,Const,Yarn,Width,Tags,Keterangan";
            string content2 = "FABRIC,FA,FABRIC,MT,Komposisi,Const,Yarn,Width,Tags,Keterangan";

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
