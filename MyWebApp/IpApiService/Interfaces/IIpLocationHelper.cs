namespace MyWebApp.IpApiService.Interfaces
{
    public interface IIpLocationHelper
    {
        Task<string> GetLocation();
    }
}
