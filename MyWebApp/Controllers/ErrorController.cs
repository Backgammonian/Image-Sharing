using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers
{
    public class ErrorController : Controller
    {
        private readonly TelemetryClient _telemetryClient;

        public ErrorController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        [Route("500")]
        public IActionResult AppError()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null)
            {
                _telemetryClient.TrackException(exceptionHandlerPathFeature.Error);
                _telemetryClient.TrackEvent("Error.ServerError", new Dictionary<string, string>
                {
                    ["originalPath"] = exceptionHandlerPathFeature.Path,
                    ["error"] = exceptionHandlerPathFeature.Error.Message
                });
            }

            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            var originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }

            _telemetryClient.TrackEvent("Error.PageNotFound", new Dictionary<string, string>
            {
                ["originalPath"] = originalPath ?? "Unknown error",
            });

            return View();
        }
    }
}
