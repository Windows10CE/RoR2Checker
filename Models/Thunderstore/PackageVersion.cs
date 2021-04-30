using System;
using System.IO;
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

        public ZipArchive Zip = null;

        public void Dispose()
        {
            if (Zip != null)
                Zip.Dispose();
        }

        public async Task<bool> Download(bool useCache = true)
        {
            var zipPath = Path.Combine("Cache", full_name + ".zip");

            if (useCache && File.Exists(zipPath)) {
                Zip = new ZipArchive(File.OpenRead(zipPath), ZipArchiveMode.Read, false);
                return true;
            }
            else if (File.Exists(zipPath)) {
                File.Delete(zipPath);
            }
            
            using var http = new HttpClient();

            using var zipReq = await http.SendAsync(new HttpRequestMessage()
            {
                RequestUri = new Uri(this.download_url)
            });

            if (!zipReq.IsSuccessStatusCode)
            {
                return false;
            }

            var file = File.OpenWrite(zipPath);
            await zipReq.Content.CopyToAsync(file);
            await file.DisposeAsync();
            Zip = new ZipArchive(File.OpenRead(zipPath), ZipArchiveMode.Read, false);
            return false;
        }
    }
}
