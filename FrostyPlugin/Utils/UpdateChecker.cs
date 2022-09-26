using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Cache;

namespace Frosty.Core
{
    public class Release
    {
        public string Name;

        [JsonProperty(PropertyName = "prerelease")]
        public bool IsPrerelease;
        [JsonProperty(PropertyName = "tag_name")]
        public string Tag;
        [JsonProperty(PropertyName = "target_commitish")]
        public Version Version;
    }

    public static class UpdateChecker
    {
        private static Release GetLatestRelease(string url, bool isArray = true)
        {
            WebClient client = new WebClient()
            {
                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
            };
            client.Headers.Add(HttpRequestHeader.UserAgent, "request");

            return isArray
                ? JsonConvert.DeserializeObject<List<Release>>(client.DownloadString(url))[0]
                : JsonConvert.DeserializeObject<Release>(client.DownloadString(url));
        }

        public static bool CheckVersion(bool checkPrerelease, Version localVersion)
        {
            Release release = checkPrerelease
                ? GetLatestRelease("https://api.github.com/repos/CadeEvs/FrostyToolsuite/releases")
                : GetLatestRelease("https://api.github.com/repos/CadeEvs/FrostyToolsuite/releases/latest", false);

            // check if local version isn't the latest
            /*if (localVersion.CompareTo(release.Version) < 0)
            {
                return true;
            }
            else if (checkPrerelease)
            {
                string version = release.Tag.ToString();
                int preVersion = int.Parse(version.Substring(version.Length - 1));

                // check if local beta/alpha version isn't the latest
                if (Frosty.Core.App.Version < preVersion)
                {
                    return true;
                }
            }*/

            return false;
        }
    }
}
