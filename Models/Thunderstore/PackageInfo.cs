using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace RoR2Checker.Models.Thunderstore
{
    public class PackageInfo : IDisposable
    {
        [JsonPropertyName("namespace")]
        public string pkg_namespace { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string owner { get; set; }
        public string package_url { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_updated { get; set; }
        public int rating_score { get; set; }
        public bool is_pinned { get; set; }
        public bool is_deprecated { get; set; }
        public int total_downloads { get; set; }
        public PackageVersion latest { get; set; }
        public CommunityListing[] community_listings { get; set; }

        public static async Task<PackageInfo> FromAuthorAndNameAsync(string author, string pkgName)
        {
            using var client = new HttpClient();

            return await client.GetFromJsonAsync<PackageInfo>($"https://thunderstore.io/api/experimental/package/{author}/{pkgName}/");
        }

        public void Dispose() => latest.Dispose();
    }
}
