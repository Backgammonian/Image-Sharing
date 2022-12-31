using Newtonsoft.Json;

namespace MyWebApp.Data
{
    public static class IpLocationHelper
    {
        private const string _unknownIP = "Unknown IP";

        public static async Task<string> GetLocation(HttpContext context)
        {
            try
            {
                var clientIpAddress = context.Connection.RemoteIpAddress;

                var url = $"http://ip-api.com/json/{clientIpAddress}?fields=66846719";
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var ipInfo = JsonConvert.DeserializeObject<IpApiServiceResponse>(content);
                
                if (ipInfo != null &&
                    ipInfo.Status == "success")
                {
                    return $"{ipInfo.Country}, {ipInfo.City}";
                }

                return $"{_unknownIP} ({clientIpAddress})";
            }
            catch (Exception ex)
            {
                return _unknownIP;
            }
        }
    }
}