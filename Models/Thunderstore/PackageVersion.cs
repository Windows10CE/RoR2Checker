using System;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace RoR2Checker.Models.Thunderstore
{
    public class PackageVersion : IDisposable
    {
        [JsonPropertyName("namespace")]
        public string pkg_namespace { get; set; }
        public string name { get; set; }
        public string version_number { get; set; }
        public string full_name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string[] dependencies { get; set; }
        public string download_url { get; set; }
        public int downloads { get; set; }
        public DateTime date_created { get; set; }
        public string website_url { get; set; }
        public bool is_active { get; set; }

        private HttpResponseMessage _zipReq = null;
        public ZipArchive Zip = null;

        public void Dispose()
        {
            if (Zip != null)
                Zip.Dispose();
            if (_zipReq != null)
                _zipReq.Dispose();
        }

        public async Task Download()
        {
            using var http = new HttpClient();

            _zipReq = await http.SendAsync(new HttpRequestMessage()
            {
                RequestUri = new Uri(this.download_url)
            });

            if (!_zipReq.IsSuccessStatusCode)
            {
                return;
            }

            Zip =  new ZipArchive(await _zipReq.Content.ReadAsStreamAsync());
        }
    }
}
