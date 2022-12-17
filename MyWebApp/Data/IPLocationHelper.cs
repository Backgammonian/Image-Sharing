using Newtonsoft.Json;
using System.Diagnostics;

namespace MyWebApp.Data
{
    public static class IPLocationHelper
    {
        private const string _unknownIP = "Unknown IP";

        public static async Task<string> GetLocation(HttpContext context)
        {
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                Debug.WriteLine($"(IPLocationHelper_GetLocation_remoteIpAddress) {remoteIpAddress}");

                var url = $"http://ip-api.com/json/{remoteIpAddress}?fields=66846719";
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var ipInfo = JsonConvert.DeserializeObject<IpApiResponse>(content);
                
                if (ipInfo != null &&
                    ipInfo.Status == "success")
                {
                    return $"{ipInfo.Country}, {ipInfo.City}";
                }

                return $"{_unknownIP} ({remoteIpAddress})";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"(IPLocationHelper_GetLocation) {ex}");
                return _unknownIP;
            }
        }
    }
}
