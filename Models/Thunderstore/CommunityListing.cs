using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoR2Checker.Models.Thunderstore
{
    public class CommunityListing
    {
        public bool has_nsfw_content { get; set; }
        public string[] categories { get; set; }
        public string community { get; set; }
    }
}
