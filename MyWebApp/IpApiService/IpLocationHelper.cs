using MyWebApp.Data;
using MyWebApp.IpApiService.Interfaces;
using Newtonsoft.Json;
using System.Net;

namespace MyWebApp.IpApiService
{
    public sealed class IpLocationHelper : IIpLocationHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpLocationHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetLocation()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                IPAddress clientIpAddress;
                if (httpContext != null &&
                    httpContext.Connection.RemoteIpAddress != null)
                {
                    clientIpAddress = httpContext.Connection.RemoteIpAddress;
                }
                else
                {
                    clientIpAddress = IPAddress.Loopback;
                }

                //fields=66846719 - all fields
                //fields=16401 - only status, country and city
                var url = $"http://ip-api.com/json/{clientIpAddress}?fields=16401";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var ipInfo = JsonConvert.DeserializeObject<IpApiServiceResponse>(content);

                if (ipInfo != null &&
                    ipInfo.Status == "success")
                {
                    return $"{ipInfo.Country}, {ipInfo.City}";
                }

                return $"{Constants.UnknownIP} ({clientIpAddress})";
            }
            catch (Exception)
            { 
            }

            return Constants.UnknownIP;
        }
    }
}