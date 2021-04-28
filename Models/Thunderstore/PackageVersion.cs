using System;
using System.Text.Json.Serialization;

namespace RoR2Checker.Models.Thunderstore
{
    public class PackageVersion
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
    }
}
