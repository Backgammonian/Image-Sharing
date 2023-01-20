using MyWebApp.Data;
using MyWebApp.IpApiService.Interfaces;
using Newtonsoft.Json;
using System.Net;

namespace MyWebApp.IpApiService
{
    public sealed class IpLocationHelper : IIpLocationHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IpLocationHelper> _logger;

        public IpLocationHelper(IHttpContextAccessor httpContextAccessor,
            ILogger<IpLocationHelper> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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

                _logger.LogInformation($"(IpLocationHelper) Getting info about address {clientIpAddress}...");

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
                    var answer = $"{ipInfo.Country}, {ipInfo.City}";
                    _logger.LogInformation($"(IpLocationHelper) Address {clientIpAddress} is pointing to {answer}");

                    return answer;
                }

                _logger.LogInformation($"(IpLocationHelper_Warning) Can't get info about address {clientIpAddress}");

                return $"{Constants.UnknownIP} ({clientIpAddress})";
            }
            catch (Exception)
            { 
            }

            return Constants.UnknownIP;
        }
    }
}