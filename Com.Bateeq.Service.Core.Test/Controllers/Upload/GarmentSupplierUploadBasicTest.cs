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
    public class GarmentSupplierUploadBasicTest
    {
        private const string URI = "v1/master/upload-garment-suppliers";
        private const string taxURI = "v1/master/upload-income-taxes";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public GarmentSupplierUploadBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public async Task<string> uploadTax()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Nama,Rate,Deskripsi";
            string content = $"{guid},1,Deskripsi";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(taxURI, multiContent);
            return guid;
        }

        [Fact]
        public async Task Should_Success_Upload_CSV()
        {
            var tax = await uploadTax();
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string guid = Guid.NewGuid().ToString();
            string header = "Kode,Nama Supplier,Alamat,Kontak,PIC,Import,Kena PPN,Kena PPH,Jenis PPH,Rate PPH,NPWP,Serial Number";
            string content = $"{guid},Nama Supplier,Alamat,Kontak,PIC,True,True,True,{tax},1,NPWP,Serial Number";

            var payload = Encoding.UTF8.GetBytes(header + "\n" + content);
            multiContent.Add(new ByteArrayContent(payload), "files", "data.csv"); // name must be "files"
            var response = await Client.PostAsync(URI, multiContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_Upload_CSV_Using_Memory_Stream()
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            string header = "Kode,Nama Supplier,Alamat,Kontak,PIC,Import,Kena PPN,Kena PPH,Jenis PPH,Rate PPH,NPWP,Serial Number";
            string content1 = "AAA,Nama Supplier,Alamat,Kontak,PIC,True,Kena PPN,Kena PPH,Jenis PPH,1,NPWP,Serial Number";
            string content2 = "AAA,Nama Supplier,Alamat,Kontak,PIC,True,Kena PPN,Kena PPH,Jenis PPH,1,NPWP,Serial Number";

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
